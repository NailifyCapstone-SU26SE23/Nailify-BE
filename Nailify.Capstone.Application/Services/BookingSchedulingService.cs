using Nailify.Capstone.Application.Common.Models.Scheduling;
using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Services
{
    public class BookingSchedulingService : IBookingSchedulingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookingSchedulingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<ProcedureScheduleSegment> BuildProcedureTimeline(
            List<BookingProcedure> procedures,
            TimeSpan bookingStartTime)
        {
            var result = new List<ProcedureScheduleSegment>();
            var groupedProcedures = procedures.GroupBy(x => x.BookingItemId);

            foreach (var group in groupedProcedures)
            {
                var cursor = bookingStartTime;
                foreach (var procedure in group.OrderBy(x => x.StepOrder))
                {
                    var start = cursor;
                    var end = start.Add(TimeSpan.FromMinutes(procedure.Duration));

                    result.Add(new ProcedureScheduleSegment
                    {
                        BookingProcedureId = procedure.BookingProcedureId,
                        BookingItemId = procedure.BookingItemId,
                        BookingId = procedure.BookingItem?.BookingId,
                        AssignedArtistId = procedure.AssignedArtistId,
                        StartTime = start,
                        EndTime = end,
                        ArtistBusyStart = start,
                        ArtistBusyEnd = start.Add(TimeSpan.FromMinutes(procedure.ActiveDuration)),
                        CanOverlap = procedure.CanOverlap
                    });

                    cursor = end;
                }
            }

            return result;
        }

        public async Task<bool> HasCapacityConflictAsync(
                              Guid artistId,
                              DateTime bookingDate,
                              List<ProcedureScheduleSegment> newSegments,
                              int artistCapacity,
                              Guid? excludingBookingId = null)
        {
            return await HasSimulationConflictAsync(
                artistId,
                bookingDate,
                newSegments,
                new List<ProcedureScheduleSegment>(),
                artistCapacity,
                excludingBookingId);
        }

        public async Task<bool> HasSimulationConflictAsync(
            Guid artistId,
            DateTime date,
            List<ProcedureScheduleSegment> newSegments,
            List<ProcedureScheduleSegment> simulatedSegments,
            int capacity,
            Guid? excludingBookingId = null)
        {
            var dbSegments = await _unitOfWork.BookingProcedureRepository
                .GetArtistBusySegmentsByDateAsync(artistId, date, excludingBookingId);

            var allExisting = dbSegments.Concat(simulatedSegments).ToList();

            // 1. Kiểm tra Active Capacity (Tối đa 1 công việc chủ động đồng thời)
            foreach (var newSegment in newSegments.Where(x => x.ArtistBusyEnd > x.ArtistBusyStart))
            {
                var activeOverlapCount = allExisting.Count(existing =>
                    existing.ArtistBusyEnd > existing.ArtistBusyStart &&
                    existing.ArtistBusyStart < newSegment.ArtistBusyEnd &&
                    existing.ArtistBusyEnd > newSegment.ArtistBusyStart);
                if (activeOverlapCount >= 1)
                {
                    return true; // Thợ bị trùng lịch làm việc chủ động
                }
            }

            // 2. Kiểm tra Total Capacity (Giới hạn ConcurrentCapacity của thợ)
            foreach (var newSegment in newSegments)
            {
                var conflictingTotals = allExisting.Where(existing =>
                    existing.StartTime < newSegment.EndTime &&
                    existing.EndTime > newSegment.StartTime).ToList();

                var totalOverlapCount = conflictingTotals
                    .GroupBy(existing => existing.BookingId ?? existing.BookingItemId ?? Guid.NewGuid())
                    .Count();

                if (totalOverlapCount >= capacity)
                {
                    return true;
                }
            }

            return false;
        }
        public async Task<List<BookingProcedure>> GenerateMockBookingProceduresAsync(List<BookingItemRequestDTO> items, Guid salonId)
        {
            var mockProcedures = new List<BookingProcedure>();
            int tempStepOrder = 1;

            var mockBooking = new Booking { BookingId = Guid.NewGuid() };
            var mockBookingItem = new BookingItem { BookingItemId = Guid.NewGuid(), Booking = mockBooking, BookingId = mockBooking.BookingId };

            foreach (var item in items)
            {
                // 1. Nếu là mẫu móng có sẵn (NailVariant)
                if (item.NailVariantId.HasValue)
                {
                    var activeNailProcedures = await _unitOfWork.NailProcedureRepository.GetActiveProceduresByVariantIdAsync(item.NailVariantId.Value);
                    foreach (var np in activeNailProcedures)
                    {
                        mockProcedures.Add(new BookingProcedure
                        {
                            BookingProcedureId = Guid.NewGuid(),
                            BookingItemId = mockBookingItem.BookingItemId,
                            BookingItem = mockBookingItem,
                            StepOrder = tempStepOrder++,
                            Duration = np.Procedure.Duration ?? 0,
                            ActiveDuration = np.Procedure.ActiveDuration,
                            PassiveDuration = np.Procedure.PassiveDuration,
                            CanOverlap = np.Procedure.CanOverlap
                        });
                    }
                }

                // 2. Nếu là dịch vụ lẻ (Service)
                if (item.ServiceId.HasValue)
                {
                    var service = await _unitOfWork.ServicesRepository.GetByIdAsync(item.ServiceId.Value);
                    if (service != null)
                    {
                        mockProcedures.Add(new BookingProcedure
                        {
                            BookingProcedureId = Guid.NewGuid(),
                            BookingItemId = mockBookingItem.BookingItemId,
                            BookingItem = mockBookingItem,
                            StepOrder = tempStepOrder++,
                            Duration = service.Duration,
                            ActiveDuration = service.Duration, // Mặc định dịch vụ lẻ là thợ bận toàn bộ thời gian
                            PassiveDuration = 0,
                            CanOverlap = false
                        });
                    }
                }
                // 3. Nếu là mẫu móng custom (CustomerNail)
                if (item.CustomerNailId.HasValue)
                {
                    var customNailRequest = await _unitOfWork.CustomerNailRequestRepository.GetApprovedRequestAsync(item.CustomerNailId.Value, salonId);
                    int duration = 60; // Mặc định

                    if (customNailRequest != null && customNailRequest.Duration.HasValue)
                    {
                        duration = customNailRequest.Duration.Value;
                    }
                    else
                    {
                        var customNail = await _unitOfWork.CustomerNailRepository.GetCustomerNailDetailAsync(item.CustomerNailId.Value);
                        if (customNail != null)
                        {
                            duration = customNail.Duration ?? 60;
                        }
                    }
                    mockProcedures.Add(new BookingProcedure
                    {
                        BookingProcedureId = Guid.NewGuid(),
                        BookingItemId = mockBookingItem.BookingItemId,
                        BookingItem = mockBookingItem,
                        StepOrder = tempStepOrder++,
                        Duration = duration,
                        ActiveDuration = duration,
                        PassiveDuration = 0,
                        CanOverlap = false
                    });
                }
            }
            return mockProcedures;
        }
    }
}
