using Nailify.Capstone.Application.Common.Models.Scheduling;
using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface IBookingSchedulingService
    {
        List<ProcedureScheduleSegment> BuildProcedureTimeline(List<BookingProcedure> procedures,TimeSpan bookingStartTime);

        Task<bool> HasCapacityConflictAsync(Guid artistId,DateTime bookingDate,
            List<ProcedureScheduleSegment> newSegments,
            int artistCapacity, Guid? excludingBookingId = null);
        Task<List<BookingProcedure>> GenerateMockBookingProceduresAsync(List<BookingItemRequestDTO> items, Guid salonId);
        Task<bool> HasSimulationConflictAsync(
            Guid artistId,
            DateTime date,
            List<ProcedureScheduleSegment> newSegments,
            List<ProcedureScheduleSegment> simulatedSegments,
            int capacity,
            Guid? excludingBookingId = null);
        // Tối ưu hiệu năng ko query MxN
        bool HasCapacityConflictInMemory(Guid artistId,
                                         List<ProcedureScheduleSegment> existingSegments,
                                         List<ProcedureScheduleSegment> newSegments,
                                         int capacity);
    }
}
