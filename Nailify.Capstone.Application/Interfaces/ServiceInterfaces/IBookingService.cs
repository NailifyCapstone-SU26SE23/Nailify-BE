using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingResponseDTOs;
using Nailify.Capstone.Domain.Enums;
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
        Task<ApiResult<SuggestedArtistResponseDTO>> GetRandomArtistAsync(GetRandomArtistRequestDTO request);
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
        Task<ApiResult<PagedList<BookingResponseDTO>>> GetMyBookingsAsync(Guid customerId, int pageNumber, int pageSize, DateTime? startDate = null, DateTime? endDate = null, BookingStatus? status = null);
        Task<ApiResult<PagedList<BookingResponseDTO>>> GetBookingsBySalonAsync(Guid salonId, int pageNumber, int pageSize, DateTime? startDate = null, DateTime? endDate = null, BookingStatus? status = null, string? search = null);
        Task<ApiResult<PagedList<BookingResponseDTO>>> GetBookingsByArtistAsync(Guid artistId, int pageNumber, int pageSize, DateTime? startDate = null, DateTime? endDate = null, BookingStatus? status = null, string? search = null);
        Task<ApiResult<BookingResponseDTO>> GetBookingByIdAsync(Guid bookingId);
        Task<ApiResult<BookingResponseDTO>> VerifyQrCodeAsync(string qrToken);
        Task<ApiResult<BookingResponseDTO>> CompleteServiceAsync(CompleteServiceRequestDTO request);
    }
}
