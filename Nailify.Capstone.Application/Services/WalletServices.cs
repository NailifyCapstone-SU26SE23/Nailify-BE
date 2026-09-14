using AutoMapper;
using Microsoft.Extensions.Logging;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.PaymentDTOs;
using Nailify.Capstone.Application.DTOs.RequestDTOs.WalletRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.WalletResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Services
{
    public class WalletService : IWalletService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPayOSPaymentService _payOSPaymentService;
        private readonly IMapper _mapper;
        private readonly ILogger<WalletService> _logger;
        public WalletService(
            IUnitOfWork unitOfWork,
            IPayOSPaymentService payOSPaymentService,
            IMapper mapper,
            ILogger<WalletService> logger)
        {
            _unitOfWork = unitOfWork;
            _payOSPaymentService = payOSPaymentService;
            _mapper = mapper;
            _logger = logger;
        }
        private async Task<CustomerWallet> GetOrCreateWalletEntityAsync(Guid customerId)
        {
            var wallet = await _unitOfWork.CustomerWalletRepository.GetByCustomerIdAsync(customerId);
            if (wallet == null)
            {
                wallet = new CustomerWallet
                {
                    CustomerId = customerId,
                    Balance = 0m,
                    FrozenBalance = 0m,
                    Status = WalletStatus.Active,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.CustomerWalletRepository.CreateAsync(wallet);
                await _unitOfWork.SaveChangesAsync();
            }
            return wallet;
        }
        public async Task<ApiResult<CustomerWalletSummaryDto>> GetWalletSummaryAsync(Guid customerId)
        {
            var wallet = await GetOrCreateWalletEntityAsync(customerId);
            var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(customerId);
            var summary = new CustomerWalletSummaryDto
            {
                WalletId = wallet.WalletId,
                CustomerId = wallet.CustomerId,
                Balance = wallet.Balance,
                FrozenBalance = wallet.FrozenBalance,
                Status = wallet.Status,
                LoyaltyPoints = customer?.LoyaltyPoint ?? 0,
                LifetimePoints = customer?.LifetimePoints ?? 0,
                LoyaltyTierName = customer?.LoyaltyTier?.Name ?? "Thành viên",
                CreatedAt = wallet.CreatedAt
            };
            return new ApiSuccessResult<CustomerWalletSummaryDto>(summary, "Lấy thông tin ví thành công.");
        }
        public async Task<ApiResult<string>> RequestDepositAsync(Guid customerId, decimal amount)
        {
            if (amount < 10000m)
            {
                return new ApiErrorResult<string>("Số tiền nạp tối thiểu là 10,000 VND.");
            }
            var wallet = await GetOrCreateWalletEntityAsync(customerId);
            var result = await _payOSPaymentService.CreateWalletDepositPaymentLinkAsync(wallet.WalletId, amount);
            if (!result.Success || result.Payment == null)
            {
                return new ApiErrorResult<string>(result.Message);
            }
            return new ApiSuccessResult<string>(result.Payment.PaymentUrl, "Tạo link nạp tiền qua PayOS thành công!");
        }
        public async Task<ApiResult<WithdrawalRequestResponseDto>> RequestWithdrawalAsync(Guid customerId, CreateWithdrawalRequestDto request)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var wallet = await _unitOfWork.CustomerWalletRepository.GetByCustomerIdForUpdateAsync(customerId);
                if (wallet == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return new ApiErrorResult<WithdrawalRequestResponseDto>("Không tìm thấy ví của khách hàng.");
                }

                var availableBalance = wallet.Balance - wallet.FrozenBalance;
                if (availableBalance < request.Amount)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return new ApiErrorResult<WithdrawalRequestResponseDto>($"Số dư khả dụng không đủ. Số dư hiện có: {availableBalance:N0} VND.");
                }
                // Khóa tạm thời tiền rút vào FrozenBalance
                wallet.FrozenBalance += request.Amount;
                wallet.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.CustomerWalletRepository.Update(wallet);
                var withdrawalRequest = new WithdrawalRequest
                {
                    CustomerId = customerId,
                    WalletId = wallet.WalletId,
                    Amount = request.Amount,
                    BankCode = request.BankCode,
                    BankName = request.BankName,
                    AccountNumber = request.AccountNumber,
                    AccountHolderName = request.AccountHolderName,
                    Status = WithdrawalStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.WithdrawalRequestRepository.CreateAsync(withdrawalRequest);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
                var response = _mapper.Map<WithdrawalRequestResponseDto>(withdrawalRequest);
                return new ApiSuccessResult<WithdrawalRequestResponseDto>(response, "Tạo yêu cầu rút tiền thành công! Đang chờ Admin duyệt.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Lỗi khi tạo yêu cầu rút tiền cho customer {CustomerId}.", customerId);
                return new ApiErrorResult<WithdrawalRequestResponseDto>($"Lỗi xử lý rút tiền: {ex.Message}");
            }
        }
        public async Task<ApiResult<string>> ConvertMoneyToPointsAsync(Guid customerId, decimal moneyAmount)
        {
            if (moneyAmount <= 0 || moneyAmount % 10000m != 0)
            {
                return new ApiErrorResult<string>("Số tiền quy đổi phải là bội số của 10,000 VND.");
            }
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var wallet = await _unitOfWork.CustomerWalletRepository.GetByCustomerIdForUpdateAsync(customerId);
                if (wallet == null || (wallet.Balance - wallet.FrozenBalance) < moneyAmount)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return new ApiErrorResult<string>("Số dư ví không đủ để thực hiện quy đổi.");
                }

                decimal conversionRate = 100m; // 100 VND = 1 point
                var pointsEarned = (int)(moneyAmount / conversionRate);
                var balanceBefore = wallet.Balance;
                wallet.Balance -= moneyAmount;
                wallet.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.CustomerWalletRepository.Update(wallet);
                var walletTx = new WalletTransaction
                {
                    WalletId = wallet.WalletId,
                    Amount = -moneyAmount,
                    BalanceBefore = balanceBefore,
                    BalanceAfter = wallet.Balance,
                    Type = WalletTransactionType.ConvertToPoints,
                    Status = WalletTransactionStatus.Completed,
                    ReferenceType = WalletReferenceType.PointsConversion,
                    Description = $"Đổi {moneyAmount:N0} VND sang {pointsEarned} điểm thưởng",
                    CreatedAt = DateTime.UtcNow
                };
                var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(customerId);
                if (customer == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return new ApiErrorResult<string>("Không tìm thấy thông tin khách hàng.");
                }
                customer.LoyaltyPoint += pointsEarned;
                customer.LifetimePoints += pointsEarned;
                _unitOfWork.CustomerRepository.Update(customer);
                var loyaltyTx = new LoyaltyTransaction
                {
                    CustomerId = customerId,
                    Points = pointsEarned,
                    TransactionType = LoyaltyTransactionType.Earned,
                    Description = $"Quy đổi từ {moneyAmount:N0} VND sang {pointsEarned} điểm thưởng",
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.WalletTransactionRepository.CreateAsync(walletTx);
                await _unitOfWork.LoyaltyTransactionRepository.CreateAsync(loyaltyTx);
                await _unitOfWork.SaveChangesAsync();
                var log = new PointConversionLog
                {
                    CustomerId = customerId,
                    WalletTransactionId = walletTx.WalletTransactionId,
                    LoyaltyTransactionId = loyaltyTx.LoyaltyTransactionId,
                    MoneyAmount = moneyAmount,
                    PointsEarned = pointsEarned,
                    ConversionRate = conversionRate,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.PointConversionLogRepository.CreateAsync(log);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
                return new ApiSuccessResult<string>($"Thành công quy đổi {moneyAmount:N0} VND sang {pointsEarned} điểm thưởng!");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Lỗi khi quy đổi tiền sang điểm cho customer {CustomerId}.", customerId);
                return new ApiErrorResult<string>($"Lỗi quy đổi: {ex.Message}");
            }
        }
        public async Task<ApiResult<PagedList<WalletTransactionResponseDto>>> GetTransactionHistoryAsync(Guid customerId, int pageNumber, int pageSize)
        {
            var wallet = await GetOrCreateWalletEntityAsync(customerId);
            var pagedTx = await _unitOfWork.WalletTransactionRepository.GetPagedByWalletIdAsync(wallet.WalletId, pageNumber, pageSize);
            var dtos = _mapper.Map<List<WalletTransactionResponseDto>>(pagedTx.Items);
            var response = new PagedList<WalletTransactionResponseDto>(dtos, pagedTx.MetaData.TotalItems, pageNumber, pageSize);
            return new ApiSuccessResult<PagedList<WalletTransactionResponseDto>>(response, "Lấy lịch sử giao dịch ví thành công.");
        }
        public async Task<ApiResult<PagedList<WithdrawalRequestResponseDto>>> GetPendingWithdrawalsAsync(int pageNumber, int pageSize)
        {
            var pagedRequests = await _unitOfWork.WithdrawalRequestRepository.GetPagedPendingAsync(pageNumber, pageSize);
            var dtos = _mapper.Map<List<WithdrawalRequestResponseDto>>(pagedRequests.Items);
            var response = new PagedList<WithdrawalRequestResponseDto>(dtos, pagedRequests.MetaData.TotalItems, pageNumber, pageSize);
            return new ApiSuccessResult<PagedList<WithdrawalRequestResponseDto>>(response, "Lấy danh sách yêu cầu rút tiền đang chờ xử lý thành công.");
        }
        public async Task<ApiResult<PagedList<WithdrawalRequestResponseDto>>> GetAllWithdrawalsAsync(WithdrawalStatus? status, int pageNumber, int pageSize)
        {
            var pagedRequests = await _unitOfWork.WithdrawalRequestRepository.GetPagedWithdrawalsAsync(status, pageNumber, pageSize);
            var dtos = _mapper.Map<List<WithdrawalRequestResponseDto>>(pagedRequests.Items);
            var response = new PagedList<WithdrawalRequestResponseDto>(dtos, pagedRequests.MetaData.TotalItems, pageNumber, pageSize);
            return new ApiSuccessResult<PagedList<WithdrawalRequestResponseDto>>(response, "Lấy danh sách yêu cầu rút tiền toàn hệ thống thành công.");
        }
        public async Task<ApiResult<PagedList<WalletTransactionResponseDto>>> GetSystemTransactionHistoryAsync(
            WalletTransactionType? type,
            WalletTransactionStatus? status,
            DateTime? fromDate,
            DateTime? toDate,
            int pageNumber,
            int pageSize)
        {
            var pagedTx = await _unitOfWork.WalletTransactionRepository.GetPagedSystemTransactionsAsync(type, status, fromDate, toDate, pageNumber, pageSize);
            var dtos = _mapper.Map<List<WalletTransactionResponseDto>>(pagedTx.Items);
            var response = new PagedList<WalletTransactionResponseDto>(dtos, pagedTx.MetaData.TotalItems, pageNumber, pageSize);
            return new ApiSuccessResult<PagedList<WalletTransactionResponseDto>>(response, "Lấy lịch sử giao dịch ví toàn hệ thống thành công.");
        }
        public async Task<ApiResult<WithdrawalRequestResponseDto>> ApproveWithdrawalAsync(Guid adminId, Guid requestId, ApproveWithdrawalDto dto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var request = await _unitOfWork.WithdrawalRequestRepository.GetByIdAsync(requestId);
                if (request == null || request.Status != WithdrawalStatus.Pending)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return new ApiErrorResult<WithdrawalRequestResponseDto>("Yêu cầu rút tiền không tồn tại hoặc đã được xử lý.");
                }
                var wallet = await _unitOfWork.CustomerWalletRepository.GetByCustomerIdForUpdateAsync(request.CustomerId);
                if (wallet == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return new ApiErrorResult<WithdrawalRequestResponseDto>("Không tìm thấy ví của khách hàng.");
                }
                var balanceBefore = wallet.Balance;
                wallet.Balance -= request.Amount;
                wallet.FrozenBalance -= request.Amount;
                wallet.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.CustomerWalletRepository.Update(wallet);

                request.Status = WithdrawalStatus.Approved;
                request.AdminNote = dto.AdminNote;
                request.TransactionReference = dto.TransactionReference;
                request.ProcessedAt = DateTime.UtcNow;
                request.ApprovedByUserId = adminId;
                _unitOfWork.WithdrawalRequestRepository.Update(request);
                var walletTx = new WalletTransaction
                {
                    WalletId = wallet.WalletId,
                    Amount = -request.Amount,
                    BalanceBefore = balanceBefore,
                    BalanceAfter = wallet.Balance,
                    Type = WalletTransactionType.Withdraw,
                    Status = WalletTransactionStatus.Completed,
                    ReferenceId = request.WithdrawalRequestId.ToString(),
                    ReferenceType = WalletReferenceType.Withdrawal,
                    Description = $"Rút tiền về tài khoản ngân hàng {request.BankCode} - {request.AccountNumber}",
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.WalletTransactionRepository.CreateAsync(walletTx);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
                var responseDto = _mapper.Map<WithdrawalRequestResponseDto>(request);
                return new ApiSuccessResult<WithdrawalRequestResponseDto>(responseDto, "Đã duyệt thành công yêu cầu rút tiền.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Lỗi khi Admin duyệt rút tiền {RequestId}.", requestId);
                return new ApiErrorResult<WithdrawalRequestResponseDto>($"Lỗi khi duyệt rút tiền: {ex.Message}");
            }
        }
        public async Task<ApiResult<WithdrawalRequestResponseDto>> RejectWithdrawalAsync(Guid adminId, Guid requestId, RejectWithdrawalDto dto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var request = await _unitOfWork.WithdrawalRequestRepository.GetByIdAsync(requestId);
                if (request == null || request.Status != WithdrawalStatus.Pending)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return new ApiErrorResult<WithdrawalRequestResponseDto>("Yêu cầu rút tiền không tồn tại hoặc đã được xử lý.");
                }
                var wallet = await _unitOfWork.CustomerWalletRepository.GetByCustomerIdForUpdateAsync(request.CustomerId);
                if (wallet == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return new ApiErrorResult<WithdrawalRequestResponseDto>("Không tìm thấy ví của khách hàng.");
                }
                // Giải phóng tiền đóng băng lại về số dư khả dụng
                wallet.FrozenBalance -= request.Amount;
                wallet.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.CustomerWalletRepository.Update(wallet);

                request.Status = WithdrawalStatus.Rejected;
                request.AdminNote = dto.AdminNote;
                request.ProcessedAt = DateTime.UtcNow;
                request.ApprovedByUserId = adminId;
                _unitOfWork.WithdrawalRequestRepository.Update(request);
 
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
                var response = _mapper.Map<WithdrawalRequestResponseDto>(request);
                return new ApiSuccessResult<WithdrawalRequestResponseDto>(response, "Đã từ chối yêu cầu rút tiền và hoàn trả số dư đóng băng.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Lỗi khi Admin từ chối rút tiền {RequestId}.", requestId);
                return new ApiErrorResult<WithdrawalRequestResponseDto>($"Lỗi khi từ chối rút tiền: {ex.Message}");
            }
        }
        public async Task<ApiResult<SystemWalletSummaryDto>> GetSystemWalletSummaryAsync()
        {
            var summary = await _unitOfWork.CustomerWalletRepository.GetSystemSummaryAsync();
            return new ApiSuccessResult<SystemWalletSummaryDto>(summary, "Lấy thống kê ví toàn hệ thống thành công.");
        }
    }
}
