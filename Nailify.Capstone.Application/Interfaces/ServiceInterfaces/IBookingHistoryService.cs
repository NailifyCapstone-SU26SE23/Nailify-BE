using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingResponseDTOs;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface IBookingHistoryService
    {
        Task<ApiResult<PagedList<BookingHistoryResponseDTO>>> GetPagedBookingHistoriesAsync(int pageNumber, int pageSize, DateTime? startDate = null, DateTime? endDate = null);
        Task<ApiResult<BookingHistoryResponseDTO>> GetBookingHistoryByIdAsync(Guid bookingHistoryId);
        Task<ApiResult<PagedList<BookingHistoryResponseDTO>>> GetPagedBookingHistoriesByBookingIdAsync(Guid bookingId, int pageNumber, int pageSize, DateTime? startDate = null, DateTime? endDate = null);
        Task<ApiResult<PagedList<BookingHistoryResponseDTO>>> GetPagedBookingHistoriesBySalonIdAsync(Guid salonId, int pageNumber, int pageSize, DateTime? startDate = null, DateTime? endDate = null);
        Task<ApiResult<PagedList<BookingHistoryResponseDTO>>> GetPagedBookingHistoriesByArtistIdAsync(Guid artistId, int pageNumber, int pageSize, DateTime? startDate = null, DateTime? endDate = null);
    }
}
