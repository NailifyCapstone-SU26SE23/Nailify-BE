using Nailify.Capstone.Application.Common;
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

        private static TransactionResponseDto Map(Transaction transaction)
        {
            return new TransactionResponseDto
            {
                TransactionId = transaction.TransactionId,
                BookingId = (Guid)transaction.BookingId,
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
                CustomerId = transaction.Booking.CustomerId,
                CustomerName = transaction.Booking.Customer?.User == null
                    ? string.Empty
                    : $"{transaction.Booking.Customer.User.FirstName} {transaction.Booking.Customer.User.LastName}".Trim(),
                SalonId = transaction.Booking.SalonId,
                SalonName = transaction.Booking.Salon?.Name ?? string.Empty
            };
        }
    }
}
