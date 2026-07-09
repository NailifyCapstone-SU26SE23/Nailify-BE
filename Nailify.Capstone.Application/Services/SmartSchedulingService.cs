using Microsoft.Extensions.Logging;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace Nailify.Capstone.Application.Services
{
    public class SmartSchedulingService : ISmartSchedulingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDurationPredictionService _durationPredictionService;
        private readonly ILogger<SmartSchedulingService> _logger;

        public SmartSchedulingService(IUnitOfWork unitOfWork, IDurationPredictionService durationPredictionService, ILogger<SmartSchedulingService> logger)
        {
            _unitOfWork = unitOfWork;
            _durationPredictionService = durationPredictionService;
            _logger = logger;
        }

        public async Task<ApiResult<List<SmartSlotDto>>> GetSmartSlotAsync(Guid salonId, DateTime date, List<BookingProcedure> procedures)
        {
            try
            {
                var recommendedSlots = new List<SmartSlotDto>();

                var salon = await _unitOfWork.SalonRepository.GetSalonWithOperatingHoursAsync(salonId);
                if (salon == null)
                {
                    return new ApiErrorResult<List<SmartSlotDto>>("Không tìm thấy thông tin salon.");
                }
                var dayOfWeek = (int)date.DayOfWeek;
                var operatingHour = salon.OperatingHours.FirstOrDefault(x => x.DayOfWeek == dayOfWeek && !x.IsClosed);
                if (operatingHour == null)
                {
                    return new ApiSuccessResult<List<SmartSlotDto>>(new List<SmartSlotDto>(), "Salon đóng cửa vào ngày này.");
                }

                // Lấy thông tin các NailVariant trong booking để kiểm tra kỹ năng yêu cầu
                var bookingItems = procedures.Select(x => x.BookingItem).Distinct().ToList();
                var variantIds = bookingItems.Where(x => x.NailVariantId.HasValue).Select(x => x.NailVariantId!.Value).ToList();

                List<NailArtist> qualifiedArtist;
                if (variantIds.Count > 0)
                {
                    qualifiedArtist = await _unitOfWork.NailArtistRepository.GetSuggestedArtistsAsync(salonId, variantIds);
                }
                else
                {
                    var allSalonArtists = await _unitOfWork.NailArtistRepository.GetNailArtistsBySalonIdAsync(salonId);
                    qualifiedArtist = allSalonArtists.ToList();
                }
                if (!qualifiedArtist.Any())
                {
                    return new ApiSuccessResult<List<SmartSlotDto>>(new List<SmartSlotDto>(), "Không có thợ nào đủ trình độ kỹ năng để thực hiện mẫu móng này.");
                }
                float requiredComplexity = 10f;
                if (variantIds.Count > 0)
                {
                    var allRequiredSkills = await _unitOfWork.NailRequiredSkillRepository.GetSkillsByVariantIdsAsync(variantIds);
                    var filteredSkills = allRequiredSkills.Where(x => !x.SkillType.Name.ToUpper().Contains("SPEED") && !x.SkillType.Name.Contains("Tốc độ"))
                        .ToList();

                    if (filteredSkills.Any())
                    {
                        requiredComplexity = filteredSkills.Sum(x => x.RequiredLevel);
                    }
                }
                foreach (var artist in qualifiedArtist)
                {
                    var artistSchedule = await _unitOfWork.ScheduleRepository.GetScheduleByArtistAndDateAsync(artist.NailArtistId, date);
                    if (artistSchedule == null)
                    {
                        continue;
                    }
                    float artistSpeed = 3f;
                    var speedSkill = artist.NailArtistSkills?.FirstOrDefault(x => x.SkillType.Name.ToUpper().Contains("SPEED") || x.SkillType.Name.Contains("Tốc độ"));
                    if (speedSkill != null)
                    {
                        artistSpeed = speedSkill.Level;
                    }
                    float baseDuration = procedures.Sum(x => x.Duration);
                    float stepsCount = procedures.Count;

                    float predictedMinutes = _durationPredictionService.PredictDuration(stepsCount, requiredComplexity, artistSpeed, baseDuration);
                    TimeSpan finalDuration = TimeSpan.FromMinutes(Math.Round(predictedMinutes));

                    // Lay lich ban cua tho trong ngay
                    var busySegments = await _unitOfWork.BookingProcedureRepository.GetArtistBusySegmentsByDateAsync(artist.NailArtistId, date);

                    var currentTime = artistSchedule.ShiftStart;
                    while (currentTime + finalDuration <= artistSchedule.ShiftEnd)
                    {
                        var slotStart = currentTime;
                        var slotEnd = currentTime + finalDuration;

                        bool isOverlap = busySegments.Any(x => x.StartTime < slotEnd && x.EndTime > slotStart);
                        if (!isOverlap)
                        {
                            double score = 0;
                            string reason = "Khung giờ trống thông thường";
                            // Heuristic 1: Tối ưu ca bận liền kề của thợ (giảm thiểu thời gian rảnh vụn)
                            bool isAdjacent = busySegments.Any(x => x.EndTime == slotStart || x.StartTime == slotEnd);
                            if (isAdjacent)
                            {
                                score += 50;
                                reason = "Tối ưu hóa lịch thợ (xếp liền kề ca bận)";
                            }

                            bool isGoldenHour = (slotStart >= new TimeSpan(11, 0, 0) && slotStart <= new TimeSpan(13, 0, 0)) ||
                                                                            (slotStart >= new TimeSpan(17, 0, 0) && slotStart <= new TimeSpan(19, 0, 0));
                            if (isGoldenHour)
                            {
                                score += 20;
                                if (!isAdjacent) reason = "Khung giờ vàng phổ biến";
                            }

                            recommendedSlots.Add(new SmartSlotDto
                            {
                                StartTime = slotStart,
                                EndTime = slotEnd,
                                AssignedArtistId = artist.NailArtistId,
                                ArtistName = $"{artist.Account.FirstName} {artist.Account.LastName}",
                                PriorityScore = score,
                                RecommendationReason = reason
                            });
                        }
                        currentTime = currentTime.Add(TimeSpan.FromMinutes(15));
                    }
                }
                // Trả về danh sách được xếp hạng ưu tiên cao nhất lên đầu
                var sortedSlots = recommendedSlots
                    .OrderByDescending(s => s.PriorityScore)
                    .ThenBy(s => s.StartTime)
                    .Take(15)
                    .ToList();
                return new ApiSuccessResult<List<SmartSlotDto>>(sortedSlots, "Lấy danh sách giờ hẹn thông minh thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xảy ra trong quá trình tính toán Smart Slots.");
                return new ApiErrorResult<List<SmartSlotDto>>($"Lỗi hệ thống: {ex.Message}");
            }
        }
    }
}
