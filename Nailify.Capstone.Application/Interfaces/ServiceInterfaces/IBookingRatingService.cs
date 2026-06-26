using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRatingRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingRatingResponseDTOs;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface IBookingRatingService
    {
        Task<ApiResult<PagedList<BookingRatingResponseDTO>>> GetAllAsync(BookingRatingRequestParameters parameters);
        Task<ApiResult<BookingRatingResponseDTO>> GetByIdAsync(Guid id);
        Task<ApiResult<PagedList<BookingRatingResponseDTO>>> GetByBookingIdAsync(Guid bookingId, BookingRatingRequestParameters parameters);
        Task<ApiResult<PagedList<BookingRatingResponseDTO>>> GetBySalonIdAsync(Guid salonId, BookingRatingRequestParameters parameters);
        Task<ApiResult<PagedList<BookingRatingResponseDTO>>> GetByNailArtistIdAsync(Guid nailArtistId, BookingRatingRequestParameters parameters);
        Task<ApiResult<PagedList<BookingRatingResponseDTO>>> GetByCustomerIdAsync(Guid customerId, BookingRatingRequestParameters parameters);
        Task<ApiResult<BookingRatingResponseDTO>> CreateAsync(Guid customerId, BookingRatingCreateRequest request, string? imageUrl);
        Task<ApiResult<BookingRatingResponseDTO>> UpdateAsync(Guid customerId, Guid id, BookingRatingUpdateRequest request, string? imageUrl);
        Task<ApiResult<bool>> DeleteAsync(Guid customerId, Guid id);
    }
}
