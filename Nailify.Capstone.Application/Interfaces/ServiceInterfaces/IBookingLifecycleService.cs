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
    public interface IBookingLifecycleService
    {
        Task<ApiResult<BookingResponseDTO>> VerifyQrCodeAsync(string qrToken, Guid actorId);
        Task<ApiResult<BookingResponseDTO>> CheckInBookingAsync(CheckInRequestDTO request, Guid actorId);
        Task<ApiResult<BookingResponseDTO>> ManualCheckInBookingAsync(Guid bookingId, Guid actorId);
        Task<ApiResult<BookingResponseDTO>> StartServiceAsync(Guid bookingId, Guid actorId);
        Task<ApiResult<BookingResponseDTO>> CompleteServiceAsync(CompleteServiceRequestDTO request, Guid actorId);
        Task<ApiResult<BookingResponseDTO>> CheckOutBookingAsync(CheckOutRequestDTO request, Guid actorId);
        Task<ApiResult<BookingResponseDTO>> ConfirmBookingAsync(Guid bookingId, Guid actorId);
        Task<ApiResult<BookingResponseDTO>> RejectBookingAsync(Guid bookingId, Guid actorId, RejectRequestDTO request);
        Task<ApiResult<BookingResponseDTO>> CancelBookingAsync(Guid bookingId, Guid customerId, CancelBookingRequestDTO request);
        Task<ApiResult<BookingResponseDTO>> UpdateBookingAsync(Guid bookingId, UpdateBookingRequestDTO request, Guid actorId);
        Task<ApiResult<BookingResponseDTO>> LateCheckInBookingAsync(Guid bookingId, Guid actorId);
    }
}
