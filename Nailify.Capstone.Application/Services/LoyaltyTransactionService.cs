using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.LoyaltyTransactionRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.WalletResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;

namespace Nailify.Capstone.Application.Services
{
    public class LoyaltyTransactionService : ILoyaltyTransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LoyaltyTransactionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResult<PagedList<LoyaltyTransactionDto>>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Guid? userId = null)
        {
            var transactions = await _unitOfWork.LoyaltyTransactionRepository
                .GetPagedAsync(pageNumber, pageSize, userId);
            var response = new PagedList<LoyaltyTransactionDto>(
                _mapper.Map<List<LoyaltyTransactionDto>>(transactions.Items),
                transactions.MetaData.TotalItems,
                pageNumber,
                pageSize);

            return new ApiSuccessResult<PagedList<LoyaltyTransactionDto>>(
                response,
                "Lấy lịch sử điểm thành công.");
        }

        public async Task<ApiResult<LoyaltyTransactionDto>> GetByIdAsync(int id)
        {
            var transaction = await _unitOfWork.LoyaltyTransactionRepository.GetByIdAsync(id);
            return transaction == null
                ? new ApiErrorResult<LoyaltyTransactionDto>("Không tìm thấy giao dịch điểm.")
                : new ApiSuccessResult<LoyaltyTransactionDto>(_mapper.Map<LoyaltyTransactionDto>(transaction), "Lấy giao dịch điểm thành công.");
        }

        public async Task<ApiResult<LoyaltyTransactionDto>> UpdateAsync(int id, LoyaltyTransactionUpdateRequest request)
        {
            var transaction = await _unitOfWork.LoyaltyTransactionRepository.GetByIdAsync(id);
            if (transaction == null) return new ApiErrorResult<LoyaltyTransactionDto>("Không tìm thấy giao dịch điểm.");
            if (request.TransactionType != LoyaltyTransactionType.Adjusted)
                return new ApiErrorResult<LoyaltyTransactionDto>("Giao dịch chỉnh sửa phải có loại Adjusted.");

            var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(transaction.CustomerId);
            if (customer == null) return new ApiErrorResult<LoyaltyTransactionDto>("Không tìm thấy khách hàng.");

            var pointDifference = request.Points - transaction.Points;
            if (customer.LoyaltyPoint + pointDifference < 0)
                return new ApiErrorResult<LoyaltyTransactionDto>("Điểm hiện tại của khách hàng không thể nhỏ hơn 0.");

            customer.LoyaltyPoint += pointDifference;
            if (pointDifference > 0) customer.LifetimePoints += pointDifference;
            transaction.Points = request.Points;
            transaction.TransactionType = LoyaltyTransactionType.Adjusted;

            _unitOfWork.CustomerRepository.Update(customer);
            _unitOfWork.LoyaltyTransactionRepository.Update(transaction);
            await _unitOfWork.SaveChangesAsync();
            return new ApiSuccessResult<LoyaltyTransactionDto>(_mapper.Map<LoyaltyTransactionDto>(transaction), "Cập nhật giao dịch điểm thành công.");
        }

        /// <summary>
        /// Hoàn điểm trực tiếp vào Ví Điểm của khách hàng (Dùng khi đền bù hoặc hoàn lại điểm đã tiêu)
        /// </summary>
        public async Task<ApiResult<LoyaltyTransactionDto>> RefundPointsToWalletAsync(Guid customerId, Guid? bookingId, int pointsToRefund, string reason)
        {
            if(pointsToRefund <= 0)
            {
                return new ApiErrorResult<LoyaltyTransactionDto>("Số điểm hoàn phải lớn hơn 0.");
            }
            var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(customerId);
            if(customer == null)
            {
                return new ApiErrorResult<LoyaltyTransactionDto>("Không tìm thấy thông tin khách hàng.");
            }

            // 1. Cộng điểm vào Ví Điểm Ảo
            customer.LoyaltyPoint += pointsToRefund;
            _unitOfWork.CustomerRepository.Update(customer);

            var transaction = new LoyaltyTransaction
            {
                CustomerId = customerId,
                BookingId = bookingId,
                Points = pointsToRefund,
                TransactionType = LoyaltyTransactionType.Refund,
                Description = $"Hoàn {pointsToRefund} điểm vào ví. Lý do: {reason}",
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.LoyaltyTransactionRepository.CreateAsync(transaction);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<LoyaltyTransactionDto>(transaction);
            return new ApiSuccessResult<LoyaltyTransactionDto>(response, $"Đã hoàn {pointsToRefund} điểm vào ví thành công.");
        }

        /// <summary>
        /// Thu hồi điểm tích thưởng của đơn hàng khi đơn hàng bị Hủy hoặc Hoàn trả (Revert Earned Points)
        /// </summary>
        public async Task<ApiResult<bool>> RevertEarnedPointsAsync(Guid customerId, Guid bookingId, string reason)
        {
            var existingEarnedTx = await _unitOfWork.LoyaltyTransactionRepository.GetEarnedTransactionByBookingIdAsync(bookingId);
            if (existingEarnedTx == null)
            {
                return new ApiSuccessResult<bool>(true, "Đơn hàng này chưa được tích điểm.");
            }
            var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(customerId);
            if (customer == null)
            {
                return new ApiErrorResult<bool>("Không tìm thấy thông tin khách hàng.");
            }
            int pointsToRevert = existingEarnedTx.Points;
            // Trừ cả LoyaltyPoint và LifetimePoints
            customer.LoyaltyPoint = Math.Max(0, customer.LoyaltyPoint - pointsToRevert);
            customer.LifetimePoints = Math.Max(0, customer.LifetimePoints - pointsToRevert);
            _unitOfWork.CustomerRepository.Update(customer);

            var revertTx = new LoyaltyTransaction
            {
                CustomerId = customerId,
                BookingId = bookingId,
                Points = -pointsToRevert,
                TransactionType = LoyaltyTransactionType.Reverted,
                Description = $"Thu hồi {pointsToRevert} điểm tích thưởng từ đơn #{bookingId}. Lý do: {reason}",
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.LoyaltyTransactionRepository.CreateAsync(revertTx);
            await _unitOfWork.SaveChangesAsync();
            return new ApiSuccessResult<bool>(true, $"Đã thu hồi {pointsToRevert} điểm từ đơn hàng hủy.");
        }

        public async Task<ApiResult<WalletSummaryDTO>> GetWalletSummaryAsync(Guid customerId)
        {
            var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(customerId);
            if (customer == null)
            {
                return new ApiErrorResult<WalletSummaryDTO>("Không tìm thấy thông tin khách hàng.");
            }
            var activeVouchers = await _unitOfWork.UserPromotionUsageRepository.GetValidUserVouchersAsync(customerId);
            var response = _mapper.Map<WalletSummaryDTO>(customer);
            response.AvailableVouchersCount = activeVouchers.Count;
            return new ApiSuccessResult<WalletSummaryDTO>(response, "Lấy thông tin tổng quan ví thành công.");
        }
    }
}
