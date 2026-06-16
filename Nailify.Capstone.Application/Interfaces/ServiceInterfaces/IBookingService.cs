using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingResponseDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface IBookingService
    {
        Task<ApiResult<List<SuggestedArtistResponseDTO>>> GetSuggestedArtistAsync(GetSuggestedArtistsRequestDTO request);
        Task<ApiResult<ArtistAvailabilityResponseDTO>> GetArtistAvailableSlotAsync(GetArtistAvailableSlotsRequestDTO request);
        Task<ApiResult<BookingResponseDTO>> CreateBookingAsync(Guid customerId, CreateBookingRequestDTO request);
        Task<ApiResult<BookingResponseDTO>> CreateCustomBookingAsync(Guid customerId, CreateCustomBookingRequestDTO request);
        Task<ApiResult<BookingResponseDTO>> AssignArtistAsync(Guid bookingId, AssignArtistRequestDTO request);
        Task<ApiResult<BookingResponseDTO>> ArtistQuoteAsync(Guid bookingId, ArtistQuoteRequestDTO request);
        Task<ApiResult<BookingResponseDTO>> ManagerApproveQuoteAsync(Guid bookingId, ManagerApproveQuoteRequestDTO request);
        Task<ApiResult<BookingResponseDTO>> CheckInBookingAsync(CheckInRequestDTO request);
        Task<ApiResult<BookingResponseDTO>> CheckOutBookingAsync(CheckOutRequestDTO request);
        Task<ApiResult<BookingResponseDTO>> UpdateBookingAsync(Guid bookingId, UpdateBookingRequestDTO request);
        Task<ApiResult<BookingResponseDTO>> CancelBookingAsync(Guid bookingId, Guid customerId, CancelBookingRequestDTO request);
        Task<ApiResult<BookingResponseDTO>> ConfirmBookingAsync(Guid bookingId);
        Task<ApiResult<BookingResponseDTO>> RejectBookingAsync(Guid bookingId);
        Task<ApiResult<BookingResponseDTO>> StartServiceAsync(Guid bookingId);
        Task<ApiResult<IEnumerable<BookingResponseDTO>>> GetMyBookingsAsync(Guid customerId);
        Task<ApiResult<IEnumerable<BookingResponseDTO>>> GetBookingsBySalonAsync(Guid salonId, DateTime? date = null);
        Task<ApiResult<IEnumerable<BookingResponseDTO>>> GetBookingsByArtistAsync(Guid artistId);
        Task<ApiResult<BookingResponseDTO>> GetBookingByIdAsync(Guid bookingId);
        Task<ApiResult<BookingResponseDTO>> VerifyQrCodeAsync(string qrToken);

    }
}
