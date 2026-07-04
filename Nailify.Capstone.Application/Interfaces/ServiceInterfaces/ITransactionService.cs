using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.TransactionResponseDTOs;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface ITransactionService
    {
        Task<ApiResult<PagedList<TransactionResponseDto>>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            DateTime? startDate = null,
            DateTime? endDate = null,
            TransactionStatus? status = null,
            Guid? salonId = null);
        Task<ApiResult<PagedList<TransactionResponseDto>>> GetMyPagedAsync(
            Guid customerId,
            int pageNumber,
            int pageSize,
            DateTime? startDate = null,
            DateTime? endDate = null,
            TransactionStatus? status = null);
        Task<ApiResult<TransactionResponseDto>> GetByIdAsync(int id);
        Task<ApiResult<IEnumerable<TransactionResponseDto>>> GetByBookingIdAsync(Guid bookingId);
    }
}
