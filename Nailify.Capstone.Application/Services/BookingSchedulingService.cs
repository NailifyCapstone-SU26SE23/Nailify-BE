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

            // Sắp xếp tuần tự tất cả các bước của các dịch vụ khác nhau trong cùng đơn đặt lịch
            var orderedProcedures = procedures
                .OrderBy(x => x.BookingItemId)
                .ThenBy(x => x.StepOrder)
                .ToList();

            var cursor = bookingStartTime;
            foreach (var procedure in orderedProcedures)
            {
                var start = cursor;
                var end = start.Add(TimeSpan.FromMinutes(procedure.Duration));
                var transition = procedure.TransitionBuffer > 0 ? procedure.TransitionBuffer : 1;

                result.Add(new ProcedureScheduleSegment
                {
                    BookingProcedureId = procedure.BookingProcedureId,
                    BookingItemId = procedure.BookingItemId,
                    BookingId = procedure.BookingItem?.BookingId,
                    AssignedArtistId = procedure.AssignedArtistId,
                    IsMainStep = procedure.IsMainStep,
                    StartTime = start,
                    EndTime = end,
                    ArtistBusyStart = start,
                    ArtistBusyEnd = start.Add(TimeSpan.FromMinutes(procedure.ActiveDuration + transition)),
                    CanOverlap = procedure.PassiveDuration >= 4 && procedure.CanOverlap,
                    TransitionBuffer = transition
                });

                cursor = end;
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
            return HasCapacityConflictInMemory(artistId, allExisting, newSegments, capacity);
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
                        var passiveDuration = np.Procedure.PassiveDuration;
                        mockProcedures.Add(new BookingProcedure
                        {
                            BookingProcedureId = Guid.NewGuid(),
                            BookingItemId = mockBookingItem.BookingItemId,
                            BookingItem = mockBookingItem,
                            StepOrder = tempStepOrder++,
                            Duration = np.Procedure.Duration ?? 0,
                            ActiveDuration = np.Procedure.ActiveDuration,
                            PassiveDuration = passiveDuration,
                            CanOverlap = passiveDuration >= 4 && np.Procedure.CanOverlap,
                            TransitionBuffer = np.Procedure.TransitionBuffer > 0  ? np.Procedure.TransitionBuffer : 1
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
                            CanOverlap = false,
                            TransitionBuffer = 1
                        });
                    }
                }
                // 3. Nếu là mẫu móng custom (CustomerNail)
                if (item.CustomerNailRequestId.HasValue)
                {
                    var customNailRequest = await _unitOfWork.CustomerNailRequestRepository.GetByIdAsync(item.CustomerNailRequestId.Value);
                    int duration = 60; // Mặc định

                    if (customNailRequest != null && customNailRequest.SalonId == salonId && customNailRequest.Duration.HasValue)
                    {
                        duration = customNailRequest.Duration.Value;
                    }
                    else if (customNailRequest != null)
                    {
                        var customNail = await _unitOfWork.CustomerNailRepository.GetCustomerNailDetailAsync(customNailRequest.CustomerNailId);
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
                        CanOverlap = false,
                        TransitionBuffer = 1
                    });
                }
            }
            return mockProcedures;
        }

        public bool HasCapacityConflictInMemory(Guid artistId, List<ProcedureScheduleSegment> existingSegments, List<ProcedureScheduleSegment> newSegments, int capacity)
        {
            // 1. Kiểm tra Active Capacity (Tối đa 1 công việc chủ động đồng thời cho thợ đang xét)
            var relevantNewSegments = newSegments.Where(x =>
                (x.AssignedArtistId == artistId || (!x.AssignedArtistId.HasValue && x.IsMainStep)) &&
                x.ArtistBusyEnd > x.ArtistBusyStart);

            foreach (var newSegment in relevantNewSegments)
            {
                var activeOverlapCount = existingSegments.Count(existing =>
                    existing.ArtistBusyEnd > existing.ArtistBusyStart &&
                    existing.ArtistBusyStart < newSegment.ArtistBusyEnd &&
                    existing.ArtistBusyEnd > newSegment.ArtistBusyStart);
                if (activeOverlapCount >= 1)
                {
                    return true; // Thợ bị trùng lịch làm việc chủ động
                }
            }

            // 2. Kiểm tra Total Capacity (Giới hạn ConcurrentCapacity của thợ đang xét)
            var relevantTotalSegments = newSegments.Where(x =>
                x.AssignedArtistId == artistId || (!x.AssignedArtistId.HasValue && x.IsMainStep));

            foreach (var newSegment in relevantTotalSegments)
            {
                var conflictingTotals = existingSegments.Where(existing =>
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
    }
}
