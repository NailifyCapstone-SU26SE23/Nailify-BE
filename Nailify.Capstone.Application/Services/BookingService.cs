using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.Common.Helpers;
using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs;
using Nailify.Capstone.Application.DTOs.RequestDTOs.WalkInQueueRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingResponseDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.SalonResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;


namespace Nailify.Capstone.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingCreationService _bookingCreationService;
        private readonly IBookingLifecycleService _bookingLifecycleService;
        private readonly IBookingAssignmentService _bookingAssignmentService;
        private readonly IBookingQueryService _bookingQueryService;

        public BookingService(
           IBookingCreationService bookingCreationService,
           IBookingLifecycleService bookingLifecycleService,
           IBookingAssignmentService bookingAssignmentService,
           IBookingQueryService bookingQueryService)
        {
            _bookingCreationService = bookingCreationService;
            _bookingLifecycleService = bookingLifecycleService;
            _bookingAssignmentService = bookingAssignmentService;
            _bookingQueryService = bookingQueryService;
        }
        public Task<ApiResult<BookingResponseDTO>> CreateBookingAsync(Guid customerId, CreateBookingRequestDTO request)
         => _bookingCreationService.CreateBookingAsync(customerId, request);
        public Task<ApiResult<BookingPriceResponseDTO>> CalculateBookingPriceAsync(Guid? customerId, IEnumerable<BookingItemRequestDTO> bookingItems, List<int>? selectedPromotionIds = null)
            => _bookingCreationService.CalculateBookingPriceAsync(customerId, bookingItems, selectedPromotionIds);
        public Task<ApiResult<BookingResponseDTO>> VerifyQrCodeAsync(string qrToken, Guid actorId)
            => _bookingLifecycleService.VerifyQrCodeAsync(qrToken, actorId);
        public Task<ApiResult<BookingResponseDTO>> CheckInBookingAsync(CheckInRequestDTO request, Guid actorId)
            => _bookingLifecycleService.CheckInBookingAsync(request, actorId);
        public Task<ApiResult<BookingResponseDTO>> ManualCheckInBookingAsync(Guid bookingId, Guid actorId)
            => _bookingLifecycleService.ManualCheckInBookingAsync(bookingId, actorId);
        public Task<ApiResult<BookingResponseDTO>> StartServiceAsync(Guid bookingId, Guid actorId)
            => _bookingLifecycleService.StartServiceAsync(bookingId, actorId);
        public Task<ApiResult<BookingResponseDTO>> CompleteServiceAsync(CompleteServiceRequestDTO request, Guid actorId)
            => _bookingLifecycleService.CompleteServiceAsync(request, actorId);
        public Task<ApiResult<BookingResponseDTO>> CheckOutBookingAsync(CheckOutRequestDTO request, Guid actorId)
            => _bookingLifecycleService.CheckOutBookingAsync(request, actorId);
        public Task<ApiResult<BookingResponseDTO>> ConfirmBookingAsync(Guid bookingId, Guid actorId)
            => _bookingLifecycleService.ConfirmBookingAsync(bookingId, actorId);
        public Task<ApiResult<BookingResponseDTO>> RejectBookingAsync(Guid bookingId, Guid actorId, RejectRequestDTO request)
            => _bookingLifecycleService.RejectBookingAsync(bookingId, actorId, request);
        public Task<ApiResult<BookingResponseDTO>> CancelBookingAsync(Guid bookingId, Guid customerId, CancelBookingRequestDTO request)
            => _bookingLifecycleService.CancelBookingAsync(bookingId, customerId, request);
        public Task<ApiResult<BookingResponseDTO>> UpdateBookingAsync(Guid bookingId, UpdateBookingRequestDTO request, Guid actorId)
            => _bookingLifecycleService.UpdateBookingAsync(bookingId, request, actorId);
        public Task<ApiResult<List<SuggestedArtistResponseDTO>>> GetSuggestedArtistAsync(GetSuggestedArtistsRequestDTO request)
            => _bookingAssignmentService.GetSuggestedArtistAsync(request);
        public Task<ApiResult<SuggestedArtistResponseDTO>> GetRandomArtistAsync(GetRandomArtistRequestDTO request)
            => _bookingAssignmentService.GetRandomArtistAsync(request);
        public Task<ApiResult<ArtistAvailabilityResponseDTO>> GetArtistAvailableSlotAsync(GetArtistAvailableSlotsRequestDTO request)
            => _bookingAssignmentService.GetArtistAvailableSlotAsync(request);
        public Task<ApiResult<List<SuggestedArtistResponseDTO>>> GetAvailableArtistsForBookingAsync(Guid bookingId)
            => _bookingAssignmentService.GetAvailableArtistsForBookingAsync(bookingId);
        public Task<ApiResult<BookingResponseDTO>> ReceptionistAssignArtistAsync(Guid bookingId, AssignArtistRequestDTO request, Guid actorId)
            => _bookingAssignmentService.ReceptionistAssignArtistAsync(bookingId, request, actorId);
        public Task<ApiResult<BookingResponseDTO>> AssignChairAsync(Guid bookingId, Guid chairId, Guid actorId)
            => _bookingAssignmentService.AssignChairAsync(bookingId, chairId, actorId);
        public Task<ApiResult<CustomerWaitEtaResponseDTO>> GetPreBookedCustomerWaitTimeEtaAndCompensateAsync(Guid bookingId)
            => _bookingAssignmentService.GetPreBookedCustomerWaitTimeEtaAndCompensateAsync(bookingId);
        public Task<ApiResult<PagedList<BookingResponseDTO>>> GetMyBookingsAsync(Guid customerId, int pageNumber, int pageSize, DateTime? startDate = null, DateTime? endDate = null, BookingStatus? status = null)
            => _bookingQueryService.GetMyBookingsAsync(customerId, pageNumber, pageSize, startDate, endDate, status);
        public Task<ApiResult<PagedList<BookingResponseDTO>>> GetBookingsBySalonAsync(Guid salonId, int pageNumber, int pageSize, DateTime? startDate = null, DateTime? endDate = null, BookingStatus? status = null, string? search = null)
            => _bookingQueryService.GetBookingsBySalonAsync(salonId, pageNumber, pageSize, startDate, endDate, status, search);
        public Task<ApiResult<PagedList<BookingResponseDTO>>> GetBookingsByArtistAsync(Guid artistId, int pageNumber, int pageSize, DateTime? startDate = null, DateTime? endDate = null, BookingStatus? status = null, string? search = null)
            => _bookingQueryService.GetBookingsByArtistAsync(artistId, pageNumber, pageSize, startDate, endDate, status, search);
        public Task<ApiResult<BookingIdResponseDTO>> GetBookingIdByOrderCodeAsync(long orderCode)
            => _bookingQueryService.GetBookingIdByOrderCodeAsync(orderCode);
        public Task<ApiResult<BookingResponseDTO>> GetBookingByIdAsync(Guid bookingId)
            => _bookingQueryService.GetBookingByIdAsync(bookingId);
        public Task<ApiResult<BookingResponseDTO>> GetBookingDetailWithWarrantyAsync(Guid bookingId)
           => _bookingQueryService.GetBookingDetailWithWarrantyAsync(bookingId);
        public Task<ApiResult<SalonAvailabilityResponseDTO>> GetSalonAvailableSlotsAsync(GetSalonAvailableSlotsRequestDTO request)
           => _bookingAssignmentService.GetSalonAvailableSlotsAsync(request);
        public Task<ApiResult<TransferPreviewResponseDTO>> PreviewTransferSalonAsync(Guid bookingId, Guid targetSalonId, Guid actorId)
            => _bookingAssignmentService.PreviewTransferSalonAsync(bookingId, targetSalonId, actorId);
        public Task<ApiResult<BookingResponseDTO>> TransferSalonAsync(Guid bookingId, TransferSalonRequestDTO request, Guid actorId)
            => _bookingAssignmentService.TransferSalonAsync(bookingId, request, actorId);

        public Task<ApiResult<List<BookingResponseDTO>>> GetLateCancelledBookingsBySalonAsync(Guid salonId)
            => _bookingQueryService.GetLateCancelledBookingsBySalonAsync(salonId);

        public Task<ApiResult<BookingResponseDTO>> LateCheckInBookingAsync(Guid bookingId, Guid actorId)
                 => _bookingLifecycleService.LateCheckInBookingAsync(bookingId, actorId);

        public Task<ApiResult<string>> HandleCustomerDelayDecisionAsync(Guid bookingId, DelayResponseRequest request)
                => _bookingLifecycleService.HandleCustomerDelayDecisionAsync(bookingId, request);
    }
}
