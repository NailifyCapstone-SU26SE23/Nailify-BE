using Nailify.Capstone.Application.Common.Models.Scheduling;
using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingResponseDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.NailArtistResponseDTOs;

namespace Nailify.Capstone.Application.Services
{
    public class BookingSchedulingService : IBookingSchedulingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly IPromotionService _promotionService;

        public BookingSchedulingService(
            IUnitOfWork unitOfWork, 
            INotificationService notificationService,
            IPromotionService promotionService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _promotionService = promotionService;
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

            var mockBooking = new Booking { BookingId = Guid.NewGuid() };
            var mockBookingItem = new BookingItem { BookingItemId = Guid.NewGuid(), Booking = mockBooking, BookingId = mockBooking.BookingId };

            // Query common procedures from master Procedure catalog (ProcedureType == Common or IsMainStep == true)
            var commonProcedures = _unitOfWork.ProcedureRepository.FindByCondition(
                p => p.Status == "Active" && (p.ProcedureType == ProcedureType.Common || p.IsMainStep)
            ).OrderBy(p => p.CreateAt).ToList();

            foreach (var item in items)
            {
                int currentStepOrder = 1;

                foreach (var commonProc in commonProcedures)
                {
                    var passiveDuration = commonProc.PassiveDuration;
                    mockProcedures.Add(new BookingProcedure
                    {
                        BookingProcedureId = Guid.NewGuid(),
                        BookingItemId = mockBookingItem.BookingItemId,
                        BookingItem = mockBookingItem,
                        ProcedureId = commonProc.ProcedureId,
                        ProcedureName = commonProc.Name,
                        StepOrder = currentStepOrder++,
                        Duration = commonProc.Duration ?? 10,
                        ActiveDuration = commonProc.ActiveDuration,
                        PassiveDuration = passiveDuration,
                        CanOverlap = passiveDuration >= 4 && commonProc.CanOverlap,
                        TransitionBuffer = commonProc.TransitionBuffer > 0 ? commonProc.TransitionBuffer : 1,
                        IsRequired = commonProc.IsRequired,
                        IsMainStep = true,
                        Status = BookingProcedureStatus.Pending
                    });
                }

                if (item.NailVariantId.HasValue)
                {
                    var activeNailProcedures = (await _unitOfWork.NailProcedureRepository.GetActiveProceduresByVariantIdAsync(item.NailVariantId.Value))
                        .Where(np => !commonProcedures.Any(cp => cp.ProcedureId == np.ProcedureId))
                        .ToList();

                    if (activeNailProcedures.Any())
                    {
                        var variant = await _unitOfWork.NailVariantRepository.GetByIdAsync(item.NailVariantId.Value);
                        int targetDuration = variant?.Duration ?? 0;
                        int totalCatalogDuration = activeNailProcedures.Sum(x => x.Procedure.Duration ?? 0);

                        if (targetDuration > 0 && totalCatalogDuration > 0 && targetDuration != totalCatalogDuration)
                        {
                            double scaleFactor = (double)targetDuration / totalCatalogDuration;
                            int accumulatedDuration = 0;
                            int count = activeNailProcedures.Count;

                            for (int i = 0; i < count; i++)
                            {
                                var np = activeNailProcedures[i];
                                int catalogDuration = np.Procedure.Duration ?? 0;
                                int scaledDuration = (int)Math.Max(1, Math.Round(catalogDuration * scaleFactor));

                                if (i == count - 1)
                                {
                                    scaledDuration = Math.Max(1, targetDuration - accumulatedDuration);
                                }
                                else
                                {
                                    accumulatedDuration += scaledDuration;
                                }

                                int scaledActive = (int)Math.Min(scaledDuration, Math.Max(1, Math.Round(np.Procedure.ActiveDuration * scaleFactor)));
                                int scaledPassive = Math.Max(0, scaledDuration - scaledActive);

                                mockProcedures.Add(new BookingProcedure
                                {
                                    BookingProcedureId = Guid.NewGuid(),
                                    BookingItemId = mockBookingItem.BookingItemId,
                                    BookingItem = mockBookingItem,
                                    ProcedureId = np.ProcedureId,
                                    ProcedureName = np.Procedure.Name,
                                    StepOrder = currentStepOrder++,
                                    Duration = scaledDuration,
                                    ActiveDuration = scaledActive,
                                    PassiveDuration = scaledPassive,
                                    CanOverlap = scaledPassive >= 4 && np.Procedure.CanOverlap,
                                    TransitionBuffer = np.Procedure.TransitionBuffer > 0 ? np.Procedure.TransitionBuffer : 1,
                                    IsRequired = np.Procedure.IsRequired,
                                    IsMainStep = np.Procedure.IsMainStep,
                                    Status = BookingProcedureStatus.Pending
                                });
                            }
                        }
                        else
                        {
                            foreach (var np in activeNailProcedures)
                            {
                                var passiveDuration = np.Procedure.PassiveDuration;
                                mockProcedures.Add(new BookingProcedure
                                {
                                    BookingProcedureId = Guid.NewGuid(),
                                    BookingItemId = mockBookingItem.BookingItemId,
                                    BookingItem = mockBookingItem,
                                    ProcedureId = np.ProcedureId,
                                    ProcedureName = np.Procedure.Name,
                                    StepOrder = currentStepOrder++,
                                    Duration = np.Procedure.Duration ?? 15,
                                    ActiveDuration = np.Procedure.ActiveDuration,
                                    PassiveDuration = passiveDuration,
                                    CanOverlap = passiveDuration >= 4 && np.Procedure.CanOverlap,
                                    TransitionBuffer = np.Procedure.TransitionBuffer > 0 ? np.Procedure.TransitionBuffer : 1,
                                    IsRequired = np.Procedure.IsRequired,
                                    IsMainStep = np.Procedure.IsMainStep,
                                    Status = BookingProcedureStatus.Pending
                                });
                            }
                        }
                    }
                }

                // 2. Nếu là dáng móng (ShapeMethodConfig)
                if (item.ShapeMethodConfigId.HasValue)
                {
                    var shapeMethodConfig = await _unitOfWork.ShapeMethodConfigRepository.GetByIdAsync(item.ShapeMethodConfigId.Value);
                    if (shapeMethodConfig != null)
                    {
                        mockProcedures.Add(new BookingProcedure
                        {
                            BookingProcedureId = Guid.NewGuid(),
                            BookingItemId = mockBookingItem.BookingItemId,
                            BookingItem = mockBookingItem,
                            ProcedureName = $"Tạo dáng & làm móng: {shapeMethodConfig.Name}",
                            StepOrder = currentStepOrder++,
                            Duration = shapeMethodConfig.Duration,
                            ActiveDuration = shapeMethodConfig.Duration,
                            PassiveDuration = 0,
                            CanOverlap = false,
                            TransitionBuffer = 1
                        });
                    }
                }

                // 3. Nếu là dịch vụ lẻ (Service)
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
                            ProcedureName = service.Name,
                            StepOrder = currentStepOrder++,
                            Duration = service.Duration,
                            ActiveDuration = service.Duration,
                            PassiveDuration = 0,
                            CanOverlap = false,
                            TransitionBuffer = 1
                        });
                    }
                }

                // 4. Nếu là mẫu móng custom (CustomerNail)
                if (item.CustomerNailRequestId.HasValue)
                {
                    var customNailRequest = await _unitOfWork.CustomerNailRequestRepository.GetByIdAsync(item.CustomerNailRequestId.Value);
                    int duration = 60;

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
                            var customProcs = (await _unitOfWork.NailProcedureRepository.GetActiveProceduresByCustomerNailIdAsync(customNail.CustomerNailId))
                                .Where(np => !commonProcedures.Any(cp => cp.ProcedureId == np.ProcedureId))
                                .ToList();

                            if (customProcs.Any())
                            {
                                int totalCatalogDuration = customProcs.Sum(x => x.Procedure.Duration ?? 0);
                                if (duration > 0 && totalCatalogDuration > 0 && duration != totalCatalogDuration)
                                {
                                    double scaleFactor = (double)duration / totalCatalogDuration;
                                    int accumulatedDuration = 0;
                                    int count = customProcs.Count;

                                    for (int i = 0; i < count; i++)
                                    {
                                        var np = customProcs[i];
                                        int catalogDuration = np.Procedure.Duration ?? 0;
                                        int scaledDuration = (int)Math.Max(1, Math.Round(catalogDuration * scaleFactor));

                                        if (i == count - 1)
                                        {
                                            scaledDuration = Math.Max(1, duration - accumulatedDuration);
                                        }
                                        else
                                        {
                                            accumulatedDuration += scaledDuration;
                                        }

                                        int scaledActive = (int)Math.Min(scaledDuration, Math.Max(1, Math.Round(np.Procedure.ActiveDuration * scaleFactor)));
                                        int scaledPassive = Math.Max(0, scaledDuration - scaledActive);

                                        mockProcedures.Add(new BookingProcedure
                                        {
                                            BookingProcedureId = Guid.NewGuid(),
                                            BookingItemId = mockBookingItem.BookingItemId,
                                            BookingItem = mockBookingItem,
                                            ProcedureId = np.ProcedureId,
                                            ProcedureName = np.Procedure.Name,
                                            StepOrder = currentStepOrder++,
                                            Duration = scaledDuration,
                                            ActiveDuration = scaledActive,
                                            PassiveDuration = scaledPassive,
                                            CanOverlap = scaledPassive >= 4 && np.Procedure.CanOverlap,
                                            TransitionBuffer = np.Procedure.TransitionBuffer > 0 ? np.Procedure.TransitionBuffer : 1,
                                            IsRequired = np.Procedure.IsRequired,
                                            IsMainStep = np.Procedure.IsMainStep,
                                            Status = BookingProcedureStatus.Pending
                                        });
                                    }
                                }
                                else
                                {
                                    foreach (var np in customProcs)
                                    {
                                        var passiveDuration = np.Procedure.PassiveDuration;
                                        mockProcedures.Add(new BookingProcedure
                                        {
                                            BookingProcedureId = Guid.NewGuid(),
                                            BookingItemId = mockBookingItem.BookingItemId,
                                            BookingItem = mockBookingItem,
                                            ProcedureId = np.ProcedureId,
                                            ProcedureName = np.Procedure.Name,
                                            StepOrder = currentStepOrder++,
                                            Duration = np.Procedure.Duration ?? 15,
                                            ActiveDuration = np.Procedure.ActiveDuration,
                                            PassiveDuration = passiveDuration,
                                            CanOverlap = passiveDuration >= 4 && np.Procedure.CanOverlap,
                                            TransitionBuffer = np.Procedure.TransitionBuffer > 0 ? np.Procedure.TransitionBuffer : 1,
                                            IsRequired = np.Procedure.IsRequired,
                                            IsMainStep = np.Procedure.IsMainStep,
                                            Status = BookingProcedureStatus.Pending
                                        });
                                    }
                                }
                            }
                        }
                    }

                    // Fallback if no specific custom procedures linked yet
                    if (!mockProcedures.Any(p => p.StepOrder > commonProcedures.Count))
                    {
                        mockProcedures.Add(new BookingProcedure
                        {
                            BookingProcedureId = Guid.NewGuid(),
                            BookingItemId = mockBookingItem.BookingItemId,
                            BookingItem = mockBookingItem,
                            ProcedureName = "Gia công & Hoàn thiện mẫu Customize",
                            StepOrder = currentStepOrder++,
                            Duration = duration,
                            ActiveDuration = duration,
                            PassiveDuration = 0,
                            CanOverlap = false,
                            TransitionBuffer = 1
                        });
                    }
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

        public async Task HandleOverlappingOnCheckInAsync(Booking checkedInBooking)
        {
            if (checkedInBooking.NailArtistId == null)
            {
                return;
            }
            var artistId = checkedInBooking.NailArtistId.Value;

            // 1. Tìm xem Thợ hiện tại đang làm cho đơn nào khác không
            var currentBusyBooking = await _unitOfWork.BookingRepository
                .GetCurrentBusyBookingWithProceduresAsync(artistId, checkedInBooking.BookingId, checkedInBooking.BookingDate.Date);

            if (currentBusyBooking == null)
            {
                return; // Thợ rảnh, không bị đè ca
            }
            // 2. Lấy công đoạn (Procedure) mà Thợ đang thực hiện cho Khách B
            var activeProcedure = currentBusyBooking.BookingItems
                                                    .SelectMany(x => x.BookingProcedures)
                                                    .OrderBy(x => x.StepOrder)
                                                    .FirstOrDefault(x => x.Status == BookingProcedureStatus.InProgress);

            if (activeProcedure == null)
            {
                return;
            }
            // Nếu được overlap -> cho phép làm song song -> Không cần cảnh báo
            if (activeProcedure.CanOverlap)
            {
                return; 
            }

            // Nếu không cho phép đè ca -> Bị kẹt
            var nowTime = DateTime.UtcNow.AddHours(7).TimeOfDay;
            int delayMinutes = activeProcedure.EstimatedEndTime.HasValue 
                ? (int)(activeProcedure.EstimatedEndTime.Value - nowTime).TotalMinutes 
                : 15;
            
            // Ko trễ thì thoát
            if (delayMinutes <= 0)
            {
                return;
            }

            // Điều phối Thợ phụ
            var firstStep = checkedInBooking.BookingItems
                                                .SelectMany(x => x.BookingProcedures)
                                                .FirstOrDefault(x => x.StepOrder == 1);

            int prepDuration = firstStep != null ? firstStep.Duration : 15;

            var availableAlternativeArtist = await _unitOfWork.NailArtistRepository
                .GetAvailableAlternativeArtistAsync(checkedInBooking.SalonId, artistId, checkedInBooking.BookingDate.Date, checkedInBooking.StartTime, prepDuration);

            if (availableAlternativeArtist != null)
            {
                var procedure = await _unitOfWork.BookingProcedureRepository.GetProceduresByBookingIdAsync(checkedInBooking.BookingId, trackChanges: true);
                // Gán Thợ C vào bước Prep đầu tiên của Khách A
                var targetProcedures = procedure.OrderBy(x => x.StepOrder)
                                                .ToList();

                var firstPrepStep = targetProcedures.FirstOrDefault(x => x.IsMainStep == false || x.StepOrder == 1);
                if (firstPrepStep != null)
                {
                    firstPrepStep.AssignedArtistId = availableAlternativeArtist.NailArtistId;
                    _unitOfWork.BookingProcedureRepository.Update(firstPrepStep);
                    await _unitOfWork.SaveChangesAsync();
                    

                    await _notificationService.SendNotificationToUserAsync(
                        checkedInBooking.CustomerId.ToString(), 
                        "ArtistChanged", 
                        new { Message = $"Thợ phụ {availableAlternativeArtist.Account.FirstName} sẽ hỗ trợ làm sạch móng trước cho bạn." });
                    return;
                }
            }

            if (delayMinutes > 5)
            {
                var alternativeArtistsDto = new List<SuggestedReassignArtistDTO>();
                var activeArtists = await _unitOfWork.NailArtistRepository.GetArtistsWithSkillsBySalonIdAsync(checkedInBooking.SalonId);
                var existingProcedures = await _unitOfWork.BookingProcedureRepository.GetProceduresByBookingIdAsync(checkedInBooking.BookingId);
                var segments = BuildProcedureTimeline(existingProcedures, checkedInBooking.StartTime);

                foreach (var artist in activeArtists)
                {
                    if (artist.NailArtistId == artistId) 
                    {
                        continue;
                    }
                    bool hasConflict = await HasSimulationConflictAsync(
                        artist.NailArtistId,
                        checkedInBooking.BookingDate,
                        segments,
                        new List<ProcedureScheduleSegment>(),
                        capacity: artist.ConcurrentCapacity,
                        excludingBookingId: checkedInBooking.BookingId
                    );

                    alternativeArtistsDto.Add(new SuggestedReassignArtistDTO
                    {
                        NailArtistId = artist.NailArtistId,
                        ArtistName = artist.Account != null ? $"{artist.Account.FirstName} {artist.Account.LastName}" : "Thợ nail",
                        SkillMatchLevel = 100, // Hardcoded for SLA alert fallback
                        IsFullyAvailable = !hasConflict
                    });
                }

                var alertDto = new SlaViolationAlertDTO
                {
                    SalonId = checkedInBooking.SalonId,
                    AffectedBookingId = checkedInBooking.BookingId,
                    CustomerName = checkedInBooking.Customer?.User?.FirstName ?? "Khách hàng",
                    CurrentArtistId = artistId,
                    EstimatedDelayMinutes = delayMinutes,
                    OverrunningBookingOrQueueId = currentBusyBooking.BookingId,
                    AvailableAlternativeArtists = alternativeArtistsDto.OrderByDescending(a => a.IsFullyAvailable).ToList()
                };

                // Gửi SignalR tới Manager POS & Staff (thông qua group "Salon_{salonId}")
                await _notificationService.SendNotificationToSalonStaffAsync(
                    checkedInBooking.SalonId.ToString(), 
                    "SLA_VIOLATION_ALERT", 
                    alertDto);
            }

            string customerMessage = $"Thợ của bạn đang hoàn thiện bước cuối, dự kiến phục vụ sau {delayMinutes} phút.";

            if (delayMinutes >= 10)
            {
                var voucherResult = await _promotionService.AddVoucherForRescheduleAsync(checkedInBooking.BookingId);
                if (voucherResult != null && voucherResult.IsSucceeded)
                {
                    customerMessage += " Hệ thống đã tự động gửi tặng bạn 1 Voucher đền bù vào ví vì sự chậm trễ này. Mong bạn thông cảm!";
                }
            }

            await _notificationService.SendNotificationToUserAsync(
                checkedInBooking.CustomerId.ToString(), 
                "DelayETA", 
                new { Message = customerMessage });

            // BR-01.4: Gửi cho Màn hình Lễ tân (SalonStaff) để cập nhật ETA
            await _notificationService.SendNotificationToSalonStaffAsync(
                checkedInBooking.SalonId.ToString(), 
                "DelayETA", 
                new { Message = $"Khách hàng {checkedInBooking.Customer?.User?.FirstName} đang chờ. {customerMessage}" });
        }
    }
}
