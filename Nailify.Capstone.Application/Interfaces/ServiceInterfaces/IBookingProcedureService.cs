using Nailify.Capstone.Application.Common;
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
    }

}
