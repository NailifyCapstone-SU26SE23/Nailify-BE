using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingResponseDTOs;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface IBookingProcedureService
    {
        Task<ApiResult<List<BookingProcedureResponseDTO>>> GetProceduresByBookingItemIdAsync(Guid bookingItemId);

        Task<ApiResult<BookingProcedureResponseDTO>> UpdateProcedureStatusAsync(
            Guid bookingProcedureId,
            Guid artistId,
            BookingProcedureStatus status);

        Task DuplicateProceduresForBookingItemAsync(BookingItem item);
        // Thêm mới phương thức Claim công đoạn
        Task<ApiResult<BookingProcedureResponseDTO>> ClaimProcedureStepAsync(Guid bookingProcedureId, Guid accountId);
        Task<ApiResult<List<IdleArtistResponseDTO>>> GetAvailableArtistsForProcedureAsync(Guid bookingProcedureId);
        Task<ApiResult<List<BookingProcedureResponseDTO>>> GetArtistActiveProceduresAsync(Guid artistId);

        Task<ApiResult<List<BookingProcedureResponseDTO>>> GetClaimableProceduresAsync(Guid salonId);
        /// <summary>
        /// Hàm này dùng để xem thợ có thể làm song song (đè ca hay không)
        /// </summary>
        /// <param name="bookingId"></param>
        /// <returns></returns>
        Task<ApiResult<InterleavingOpportunityResponseDTO>> EvaluateInterleavingOpportunityAsync(Guid bookingId);
        Task<ApiResult<BookingProcedureResponseDTO>> AutoAssignSecondaryArtistForPrepAsync(Guid bookingId, Guid mainArtistId);
        Task<ApiResult<OnsiteAddonSimulationResponseDTO>> SimulateOnsiteAddonAsync(SimulateOnsiteAddonRequestDTO request);
        Task<ApiResult<List<BookingProcedureResponseDTO>>> ConfirmOnsiteAddonAsync(ConfirmOnsiteAddonRequestDTO request);
    }
}
