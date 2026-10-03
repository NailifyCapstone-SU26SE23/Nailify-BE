using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingResponseDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.TransactionResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TransactionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<PagedList<TransactionResponseDto>>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            DateTime? startDate = null,
            DateTime? endDate = null,
            TransactionStatus? status = null,
            Guid? salonId = null)
        {
            var result = await _unitOfWork.TransactionRepository.GetPagedDetailAsync(
                NormalizePageNumber(pageNumber),
                NormalizePageSize(pageSize),
                startDate,
                endDate,
                status,
                salonId);

            return ToPagedResult(result.Items, result.TotalItems, pageNumber, pageSize);
        }

        public async Task<ApiResult<PagedList<TransactionResponseDto>>> GetMyPagedAsync(
            Guid customerId,
            int pageNumber,
            int pageSize,
            DateTime? startDate = null,
            DateTime? endDate = null,
            TransactionStatus? status = null)
        {
            var result = await _unitOfWork.TransactionRepository.GetPagedDetailAsync(
                NormalizePageNumber(pageNumber),
                NormalizePageSize(pageSize),
                startDate,
                endDate,
                status,
                customerId: customerId);

            return ToPagedResult(result.Items, result.TotalItems, pageNumber, pageSize);
        }

        public async Task<ApiResult<TransactionResponseDto>> GetByIdAsync(int id)
        {
            var transaction = await _unitOfWork.TransactionRepository.GetDetailByIdAsync(id);
            return transaction == null
                ? new ApiErrorResult<TransactionResponseDto>("Không tìm thấy giao dịch.")
                : new ApiSuccessResult<TransactionResponseDto>(Map(transaction), "Lấy giao dịch thành công.");
        }

        public async Task<ApiResult<IEnumerable<TransactionResponseDto>>> GetByBookingIdAsync(Guid bookingId)
        {
            var transactions = await _unitOfWork.TransactionRepository.GetByBookingIdAsync(bookingId);
            return new ApiSuccessResult<IEnumerable<TransactionResponseDto>>(
                transactions.Select(Map),
                "Lấy giao dịch thành công.");
        }

        private static ApiResult<PagedList<TransactionResponseDto>> ToPagedResult(
            IEnumerable<Transaction> transactions,
            int totalItems,
            int pageNumber,
            int pageSize)
        {
            var normalizedPageNumber = NormalizePageNumber(pageNumber);
            var normalizedPageSize = NormalizePageSize(pageSize);
            var response = new PagedList<TransactionResponseDto>(
                transactions.Select(Map).ToList(),
                totalItems,
                normalizedPageNumber,
                normalizedPageSize);

            return new ApiSuccessResult<PagedList<TransactionResponseDto>>(
                response,
                "Lấy danh sách giao dịch thành công.");
        }

        private static int NormalizePageNumber(int pageNumber) => pageNumber < 1 ? 1 : pageNumber;

        private static int NormalizePageSize(int pageSize) => pageSize < 1 ? 10 : pageSize;

        public static string DeterminePaymentMethod(string? paymentLinkId, Guid? walletId)
        {
            if (string.Equals(paymentLinkId, "WALLET_PAYMENT", StringComparison.OrdinalIgnoreCase))
            {
                return "Ví";
            }
            if (!string.IsNullOrWhiteSpace(paymentLinkId) && walletId.HasValue)
            {
                return "Nạp tiền vào ví";
            }
            if (!string.IsNullOrWhiteSpace(paymentLinkId) && !walletId.HasValue)
            {
                return "Chuyển khoản";
            }
            if (string.IsNullOrWhiteSpace(paymentLinkId) && !walletId.HasValue)
            {
                return "Tiền mặt";
            }
            return "Ví";
        }

        private static TransactionResponseDto Map(Transaction transaction)
        {
            var booking = transaction.Booking;
            var bookingCustomerUser = booking?.Customer?.User;
            var wallet = transaction.Wallet;
            var walletCustomerUser = wallet?.Customer?.User;

            return new TransactionResponseDto
            {
                TransactionId = transaction.TransactionId,
                BookingId = transaction.BookingId,
                WalletId = transaction.WalletId,
                OrderCode = transaction.OrderCode,
                Amount = transaction.Amount,
                Reference = transaction.Reference,
                PaymentLinkId = transaction.PaymentLinkId,
                Policy = transaction.Policy,
                CheckoutUrl = transaction.CheckoutUrl,
                QrCode = transaction.QrCode,
                Status = transaction.Status,
                CreatedAt = transaction.CreatedAt,
                PaidAt = transaction.PaidAt,
                ExpiresAt = transaction.ExpiresAt,
                CustomerId = booking?.CustomerId ?? wallet?.CustomerId ?? Guid.Empty,
                CustomerName = bookingCustomerUser == null && walletCustomerUser == null
                    ? string.Empty
                    : $"{bookingCustomerUser?.FirstName ?? walletCustomerUser?.FirstName} {bookingCustomerUser?.LastName ?? walletCustomerUser?.LastName}".Trim(),
                SalonId = booking?.SalonId,
                SalonName = booking?.Salon?.Name ?? string.Empty,
                PaymentType = transaction.PaymentType,
                PaymentMethod = DeterminePaymentMethod(transaction.PaymentLinkId, transaction.WalletId)
            };
        }

        public async Task<ApiResult<IEnumerable<BookingPaymentHistoryDto>>> GetPaymentHistoryByBookingIdAsync(Guid bookingId)
        {
            var paymentHistory = new List<BookingPaymentHistoryDto>();

            var transactions = await _unitOfWork.TransactionRepository.GetByBookingIdAsync(bookingId);
            foreach (var transaction in transactions)
            {
                if (transaction.Status == TransactionStatus.Refunded)
                {
                    continue;
                }

                var paymentMethod = DeterminePaymentMethod(transaction.PaymentLinkId, transaction.WalletId);
                var defaultDesc = paymentMethod switch
                {
                    "Tiền mặt" => "Thanh toán bằng tiền mặt",
                    "Ví" => "Thanh toán bằng ví",
                    "Nạp tiền vào ví" => "Nạp tiền vào ví qua PayOS",
                    _ => "Thanh toán qua PayOS"
                };

                paymentHistory.Add(new BookingPaymentHistoryDto
                {
                    Id = transaction.TransactionId.ToString(),
                    Amount = transaction.Amount,
                    PaymentMethod = paymentMethod,
                    Status = transaction.Status.ToString(),
                    Description = !string.IsNullOrWhiteSpace(transaction.Reference) ? transaction.Reference : defaultDesc,
                    CreatedAt = transaction.CreatedAt
                });
            }

            var bookingIdStr = bookingId.ToString();
            var walletTransactions = await _unitOfWork.WalletTransactionRepository.GetWalletTransactionByBookingId(bookingIdStr);
            if (walletTransactions != null)
            {
                foreach (var wt in walletTransactions)
                {
                    paymentHistory.Add(new BookingPaymentHistoryDto
                    {
                        Id = wt.WalletTransactionId.ToString(),
                        Amount = Math.Abs(wt.Amount),
                        PaymentMethod = "Ví",
                        Status = wt.Status.ToString(),
                        Description = wt.Description ?? "Thanh toán bằng ví",
                        CreatedAt = wt.CreatedAt
                    });
                }
            }
            var response = paymentHistory.OrderByDescending(x => x.CreatedAt).ToList();
            return new ApiSuccessResult<IEnumerable<BookingPaymentHistoryDto>>(
                response,
                "Lấy lịch sử giao dịch thành công.");
        }
    }
}
