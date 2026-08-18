using AutoMapper;
using MediatR;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.Common.Models.Scheduling;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailArtistRequestDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Services
{
    public class NailArtistEmergencyService : INailArtistEmergencyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IBookingSchedulingService _schedulingService;
        private readonly INotificationService _notificationService;
        private readonly IEmailService _emailService;
        private readonly IBookingSkillMatchingService _skillMatchingService;
        private readonly IPromotionService _promotionService;

        public NailArtistEmergencyService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IBookingSchedulingService schedulingService,
            INotificationService notificationService,
            IEmailService emailService,
            IBookingSkillMatchingService skillMatchingService,
            IPromotionService promotionService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _schedulingService = schedulingService;
            _notificationService = notificationService;
            _emailService = emailService;
            _skillMatchingService = skillMatchingService;
            _promotionService = promotionService;
        }

        public async Task<EmergencyOffResultDTO> ProcessAffectedBookingsForDateAsync(Guid artistId, DateTime targetDate, string reason)
        {
            // Lấy tất cả  các lịch hẹn Approve của thợ trong ngày
            var affectedBookings = await _unitOfWork.BookingRepository.GetApprovedBookingsWithDetailsByArtistAndDateAsync(artistId, targetDate);

            var orderedBookings = affectedBookings.OrderBy(x => x.StartTime).ToList();

            var response = new EmergencyOffResultDTO
            {
                NailArtistId = artistId,
                OffDate = targetDate,
                TotalAffectedBookings = orderedBookings.Count
            };

            // Lay salon
            var salonId = orderedBookings.FirstOrDefault()?.SalonId;
            var candidateArtistsWithSchedules = new List<(NailArtist Artist, Schedule Schedule)>();
            if (salonId.HasValue)
            {
                var allSalonArtists = await _unitOfWork.NailArtistRepository.GetActiveArtistsWithSchedulesAndSkillsBySalonAsync(salonId.Value, artistId);
                foreach (var x in allSalonArtists)
                {
                    var caschedule = x.Schedules.FirstOrDefault(x => x.WorkDate.Date == targetDate);
                    if (caschedule != null)
                    {
                        candidateArtistsWithSchedules.Add((x, caschedule));
                    }
                }
            }
            foreach (var x in orderedBookings)
            {
                var procedures = (await _unitOfWork.BookingProcedureRepository.GetProceduresByBookingIdAsync(x.BookingId)).ToList();
                var timeline = _schedulingService.BuildProcedureTimeline(procedures, x.StartTime);

                var bookingEndTime = x.StartTime.Add(TimeSpan.FromMinutes(x.TotalDuration));
                bool reassigned = false;
                foreach (var (candidate, candidateSchedule) in candidateArtistsWithSchedules)
                {
                    if (x.StartTime < candidateSchedule.ShiftStart || bookingEndTime > candidateSchedule.ShiftEnd)
                    {
                        continue;
                    }

                    // Check gio nghi ca nhan
                    var candidateBreaks = candidate.NailArtistBreaks.Where(x => x.BreakDate.Date == targetDate && x.Status == ArtistBreakStatus.Approved);
                    if (candidateBreaks.Any(y => x.StartTime < y.EndTime && bookingEndTime > y.StartTime))
                    {
                        continue;
                    }
                    // Kiem tra skill theo nail va customize
                    bool hasRequriedSkills = await _skillMatchingService.HasRequiredSkillsAsync(candidate, x, artistId);
                    if (!hasRequriedSkills)
                    {
                        continue;
                    }

                    bool hasConflict = await _schedulingService.HasSimulationConflictAsync(
                        candidate.NailArtistId,
                        targetDate,
                        timeline,
                        new List<ProcedureScheduleSegment>(),
                        candidate.ConcurrentCapacity,
                        excludingBookingId: x.BookingId
                        );

                    if (!hasConflict)
                    {
                        x.NailArtistId = candidate.NailArtistId;
                        _unitOfWork.BookingRepository.Update(x);

                        var history = new BookingHistory
                        {
                            BookingId = x.BookingId,
                            EventType = "SYSTEM_EMERGENCY_OFF",
                            Payload = $"Tự động đổi thợ sang {candidate.Account?.FirstName} {candidate.Account?.LastName} (Đạt Skill Level & Tay nghề) do thợ ban đầu bận đột xuất.",
                            ActorId = null,
                            CreatedAt = DateTime.UtcNow.AddHours(7)
                        };

                        await _unitOfWork.BookingHistoryRepository.CreateAsync(history);
                        response.AutoReassignedCount++;

                        var detailDTO = _mapper.Map<EmergencyBookingHandlingDetailDTO>(x);
                        detailDTO.HandlingResult = EmergencyHandlingResult.Reassigned;
                        detailDTO.NewAssignedArtistId = candidate.NailArtistId;
                        detailDTO.NewAssignedArtistName = candidate.Account?.FirstName + " " + candidate.Account?.LastName;
                        response.ProcessingDetails.Add(detailDTO);

                        // Discard (Bien bo qua)
                        // Cố tình cho tác vụ này chạy ngầm ở background, hãy bỏ qua cảnh báo!
                        // Ko can cho cu chay ngam
                        _ = _notificationService.SendNotificationToUserAsync(
                            x.CustomerId.ToString(),
                             "Thông báo đổi thợ phụ trách",
                            $"Lịch hẹn lúc {x.StartTime:hh\\:mm} ngày {targetDate:dd/MM/yyyy} của bạn đã được chuyển sang Thợ {candidate.Account?.FirstName} {candidate.Account?.LastName} (Đạt trình độ chuyên môn tương đương/cao hơn). Khung giờ không đổi.");

                        reassigned = true;
                        break;
                    }
                }
                if (reassigned)
                {
                    continue;
                }
                // Reschedule  (+/- 30-60p)
                bool rescheduleSuggested = false;
                // Khoảng chênh lệch thời gian để tìm khung giờ rảnh gần nhất cho khách hàng khi khung giờ hẹn ban đầu đã bị kín sạch thợ               
                // Ưu tiên tìm những khung giờ gần với giờ hẹn ban đầu của khách nhất
                // Dùng Nearest Slot Search Algorithm
                var potentialOffsets = new[] { 30, -30, 60, -60 };
                foreach (var offsetMinutes in potentialOffsets)
                {
                    var suggestedStartTime = x.StartTime.Add(TimeSpan.FromMinutes(offsetMinutes));
                    var suggestedEndTime = suggestedStartTime.Add(TimeSpan.FromMinutes(x.TotalDuration));
                    foreach (var (candidate, candidateSchedule) in candidateArtistsWithSchedules)
                    {
                        if (suggestedStartTime < candidateSchedule.ShiftStart || suggestedEndTime > candidateSchedule.ShiftEnd)
                        {
                            continue;
                        }

                        bool hasRequiredSkill = await _skillMatchingService.HasRequiredSkillsAsync(candidate, x, artistId);
                        if (!hasRequiredSkill)
                        {
                            continue;
                        }

                        bool hasConflict = await _schedulingService.HasSimulationConflictAsync(
                            candidate.NailArtistId,
                            targetDate,
                            _schedulingService.BuildProcedureTimeline(procedures, suggestedStartTime),
                            new List<ProcedureScheduleSegment>(),
                            candidate.ConcurrentCapacity,
                            excludingBookingId: x.BookingId);

                        if (!hasConflict)
                        {
                            x.Status = BookingStatus.RescheduleSuggested;
                            x.ProposedBookingDate = targetDate;
                            x.ProposedStartTime = suggestedStartTime;
                            x.ProposedBy = "Manager";
                            x.RescheduleReason = $"Sự cố thợ bận đột xuất ({reason})";
                            x.NailArtistId = candidate.NailArtistId;
                            _unitOfWork.BookingRepository.Update(x);

                            // Tự động cộng Voucher đền bù dời lịch cho khách hàng
                            await _promotionService.AddVoucherForRescheduleAsync(x.BookingId);

                            response.RescheduleSuggestedCount++;

                            var detailDto = _mapper.Map<EmergencyBookingHandlingDetailDTO>(x);
                            detailDto.HandlingResult = EmergencyHandlingResult.RescheduleSuggested;
                            detailDto.SuggestedStartTime = suggestedStartTime;
                            detailDto.NewAssignedArtistId = candidate.NailArtistId;
                            detailDto.NewAssignedArtistName = candidate.Account?.FirstName + " " + candidate.Account.LastName;
                            response.ProcessingDetails.Add(detailDto);
                            _ = _notificationService.SendNotificationToUserAsync(
                                x.CustomerId.ToString(),
                                "Đề xuất thay đổi giờ hẹn",
                                $"Do sự cố thợ bận đột xuất, Salon đề xuất dời lịch của bạn sang {suggestedStartTime:hh\\:mm}. Vui lòng kiểm tra và xác nhận trên ứng dụng."
                            );
                            rescheduleSuggested = true;
                            break;
                        }
                    }
                    if (rescheduleSuggested)
                    {
                        break;
                    }
                }
                if (rescheduleSuggested)
                {
                    continue;
                }

                // LUỒNG HỦY ĐƠN & HOÀN CỌC + VOUCHER KHI KHÔNG CÓ THỢ NÀO THAY THẾ
                string cancelReason = $"[Tự động hủy] Sự cố thợ bận đột xuất ({reason}) - Không có thợ/slot có kỹ năng phù hợp thay thế.";
                x.Cancel(Guid.Empty, cancelReason);
                _unitOfWork.BookingRepository.Update(x);

                // Tự động cộng Voucher đền bù hủy đơn cho khách hàng
                await _promotionService.AddVoucherForCancelledAsync(x.BookingId);

                response.CancelledAndRefundedCount++;

                var cancelDetailDto = _mapper.Map<EmergencyBookingHandlingDetailDTO>(x);
                cancelDetailDto.HandlingResult = EmergencyHandlingResult.Cancelled;
                response.ProcessingDetails.Add(cancelDetailDto);
                _ = _notificationService.SendNotificationToUserAsync(
                    x.CustomerId.ToString(),
                    "Thông báo Hủy lịch hẹn",
                    $"Rất tiếc lịch hẹn lúc {x.StartTime:hh\\:mm} bị hủy do sự cố thợ bận đột xuất và chưa có thợ có trình độ tương đương làm mẫu móng này. Salon thành thật xin lỗi vì sự bất tiện này."
                );
            }
            return response;
        }

        public async Task<ApiResult<EmergencyOffResultDTO>> SetArtistOffDutyAsync(Guid artistId,EmergencyOffRequestDTO request)
        {
            var targetDate = (request.OffDate.Kind == DateTimeKind.Utc ? request.OffDate.AddHours(7) : request.OffDate).Date;

            var artist = await _unitOfWork.NailArtistRepository.GetByIdAsync(artistId);
            if(artist == null)
            {
                return new ApiErrorResult<EmergencyOffResultDTO>("Không tìm thấy thợ nail.");
            }

            // Ghi nhận lịch nghỉ khẩn cấp cả ngày cho Thợ X
            var schedule = await _unitOfWork.ScheduleRepository.GetScheduleByArtistAndDateAsync(artistId, targetDate);
            var shiftStart = schedule?.ShiftStart ?? new TimeSpan(8, 0, 0);
            var shiftEnd = schedule?.ShiftEnd ?? new TimeSpan(20, 0, 0);

            var emergencyBreak = new NailArtistBreak
            {
                NailArtistId = artistId,
                BreakDate = targetDate,
                StartTime = shiftStart,
                EndTime = shiftEnd,
                Reason = $"[EMERGENCY OFF] {request.Reason}",
                Status = ArtistBreakStatus.Approved
            };
            await _unitOfWork.NailArtistBreakRepository.CreateAsync(emergencyBreak);

            var response = await ProcessAffectedBookingsForDateAsync(artistId, targetDate, request.Reason);
            await _unitOfWork.SaveChangesAsync();
            return new ApiSuccessResult<EmergencyOffResultDTO>(response, "Xử lý lịch nghỉ khẩn cấp cho thợ thành công.");
        }
    }
}
