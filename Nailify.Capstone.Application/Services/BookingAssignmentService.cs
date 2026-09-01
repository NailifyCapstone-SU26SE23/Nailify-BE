using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.Common.Helpers;
using Nailify.Capstone.Application.Common.Models.Scheduling;
using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingResponseDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.SalonResponseDTOs;
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
    public class BookingAssignmentService : IBookingAssignmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly INailVariantService _nailVariantService;
        private readonly IBookingSchedulingService _bookingSchedulingService;
        private readonly ISlotHoldService _slotHoldService;
        private readonly INotificationService _notificationService;
        private readonly IBookingSkillMatchingService _skillMatchingService;
        private readonly IBookingProcedureService _bookingProcedureService;
        public BookingAssignmentService(
                                        IUnitOfWork unitOfWork,
                                        IMapper mapper,
                                        INailVariantService nailVariantService,
                                        IBookingSchedulingService bookingSchedulingService,
                                        ISlotHoldService slotHoldService,
                                        INotificationService notificationService,
                                        IBookingSkillMatchingService skillMatchingService,
                                        IBookingProcedureService bookingProcedureService
                                      )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _nailVariantService = nailVariantService;
            _bookingSchedulingService = bookingSchedulingService;
            _slotHoldService = slotHoldService;
            _notificationService = notificationService;
            _skillMatchingService = skillMatchingService;
            _bookingProcedureService = bookingProcedureService;
        }

        public async Task<ApiResult<List<SuggestedArtistResponseDTO>>> GetSuggestedArtistAsync(GetSuggestedArtistsRequestDTO request)
        {
            if (request.BookingItems == null || !request.BookingItems.Any())
            {
                return new ApiErrorResult<List<SuggestedArtistResponseDTO>>("Vui lòng chọn mẫu nail trước khi tìm thợ.");
            }
            var bookingItems = _mapper.Map<List<BookingItem>>(request.BookingItems);
            if (bookingItems.Any(item => !item.NailVariantId.HasValue && !item.ServiceId.HasValue && !item.CustomerNailRequestId.HasValue))
            {
                return new ApiErrorResult<List<SuggestedArtistResponseDTO>>("Mỗi mục đặt lịch phải chứa ít nhất một dịch vụ, một mẫu nail hoặc một mẫu custom.");
            }
            var localDate = (request.BookingDate.Kind == DateTimeKind.Utc ? request.BookingDate.AddHours(7) : request.BookingDate).Date;
            var isOffDay = await _unitOfWork.SalonOffDateRepository.ExistsAsync(x =>
                                                                                    x.SalonId == request.SalonId
                                                                                    && x.StartDate.Date <= localDate
                                                                                    && x.EndDate.Date >= localDate);
            if (isOffDay)
            {
                return new ApiErrorResult<List<SuggestedArtistResponseDTO>>("Salon đóng cửa nghỉ lễ vào ngày này.");
            }

            // Custom: Nếu là đặt lịch mẫu custom, chỉ hiển thị thợ đã duyệt báo giá mẫu này
            var customNailItem = bookingItems.FirstOrDefault(x => x.CustomerNailRequestId.HasValue);
            if (customNailItem != null)
            {
                var customNailRequest = await _unitOfWork.CustomerNailRequestRepository.GetByIdAsync(customNailItem.CustomerNailRequestId!.Value);
                if (customNailRequest != null &&
                    customNailRequest.SalonId == request.SalonId &&
                    (customNailRequest.Status == CustomerNailStatus.Approved || customNailRequest.Status == CustomerNailStatus.Quoted) &&
                    customNailRequest.ApprovedArtistId.HasValue)
                {
                    var approvedArtist = await _unitOfWork.NailArtistRepository.GetNailArtistWithProfileAsync(customNailRequest.ApprovedArtistId.Value);
                    if (approvedArtist != null && approvedArtist.Status == "Active")
                    {
                        var responseList = _mapper.Map<List<SuggestedArtistResponseDTO>>(new List<NailArtist> { approvedArtist });
                        return new ApiSuccessResult<List<SuggestedArtistResponseDTO>>(responseList, "Lấy danh sách thợ đề xuất thành công.");
                    }
                }
                return new ApiSuccessResult<List<SuggestedArtistResponseDTO>>(new List<SuggestedArtistResponseDTO>(), "Không tìm thấy thợ được chỉ định duyệt mẫu móng này.");
            }

            var variantIds = _unitOfWork.NailVariantRepository.GetDistinctVariantIdsAsync(bookingItems);
            IEnumerable<NailArtist> suggestedArtist;

            if (variantIds.Any())
            {
                suggestedArtist = await _unitOfWork.NailArtistRepository.GetSuggestedArtistsAsync(request.SalonId, variantIds);
            }
            else
            {
                var activeArtists = await _unitOfWork.NailArtistRepository.GetNailArtistsBySalonIdAsync(request.SalonId);
                suggestedArtist = activeArtists.Where(x => x.Status == "Active");
            }
            var response = _mapper.Map<List<SuggestedArtistResponseDTO>>(suggestedArtist);

            return new ApiSuccessResult<List<SuggestedArtistResponseDTO>>(response, "Lấy danh sách thợ đề xuất thành công.");
        }
        public async Task<ApiResult<SuggestedArtistResponseDTO>> GetRandomArtistAsync(GetRandomArtistRequestDTO request)
        {
            if (request.BookingItems == null || !request.BookingItems.Any())
            {
                return new ApiErrorResult<SuggestedArtistResponseDTO>("Vui lòng chọn mẫu nail trước khi tìm thợ.");
            }

            var bookingItems = _mapper.Map<List<BookingItem>>(request.BookingItems);
            if (bookingItems.Any(item => !item.NailVariantId.HasValue && !item.ServiceId.HasValue && !item.CustomerNailRequestId.HasValue))
            {
                return new ApiErrorResult<SuggestedArtistResponseDTO>("Mỗi mục đặt lịch phải chứa ít nhất một dịch vụ, một mẫu nail hoặc một mẫu custom.");
            }
            var localDate = (request.BookingDate.Kind == DateTimeKind.Utc ? request.BookingDate.AddHours(7) : request.BookingDate).Date;
            var isOffDay = await _unitOfWork.SalonOffDateRepository.ExistsAsync(x =>
                                                                                     x.SalonId == request.SalonId
                                                                                     && x.StartDate.Date <= localDate
                                                                                     && x.EndDate.Date >= localDate);
            if (isOffDay)
            {
                return new ApiErrorResult<SuggestedArtistResponseDTO>("Salon đóng cửa nghỉ lễ vào ngày này.");
            }
            int totalDuration = 0;

            foreach (var item in bookingItems)
            {
                if (item.NailVariantId.HasValue)
                {
                    var variant = await _nailVariantService.GetNailVariantByIdAsync(item.NailVariantId.Value);
                    if (variant?.Data != null)
                    {
                        totalDuration += (variant.Data.Duration ?? 60);
                    }
                }

                if (item.ServiceId.HasValue)
                {
                    var service = await _unitOfWork.ServicesRepository.GetByIdAsync(item.ServiceId.Value);
                    if (service != null)
                    {
                        totalDuration += service.Duration;
                    }
                }

                if (item.CustomerNailRequestId.HasValue)
                {
                    var customNailRequest = await _unitOfWork.CustomerNailRequestRepository.GetByIdAsync(item.CustomerNailRequestId.Value);
                    if (customNailRequest != null &&
                        customNailRequest.SalonId == request.SalonId &&
                        (customNailRequest.Status == CustomerNailStatus.Approved || customNailRequest.Status == CustomerNailStatus.Quoted))
                    {
                        var customNail = await _unitOfWork.CustomerNailRepository.GetCustomerNailDetailAsync(customNailRequest.CustomerNailId);
                        if (customNail != null)
                        {
                            totalDuration += (customNail.Duration ?? 60) + (customNailRequest.Duration ?? 0);
                        }
                    }
                }
            }

            IEnumerable<NailArtist> qualifiedArtists;
            var customNailItem = bookingItems.FirstOrDefault(x => x.CustomerNailRequestId.HasValue);

            if (customNailItem != null)
            {
                var customNailRequest = await _unitOfWork.CustomerNailRequestRepository.GetByIdAsync(customNailItem.CustomerNailRequestId!.Value);
                if (customNailRequest != null &&
                    customNailRequest.SalonId == request.SalonId &&
                    (customNailRequest.Status == CustomerNailStatus.Approved || customNailRequest.Status == CustomerNailStatus.Quoted) &&
                    customNailRequest.ApprovedArtistId.HasValue)
                {
                    var approvedArtist = await _unitOfWork.NailArtistRepository.GetNailArtistWithProfileAsync(customNailRequest.ApprovedArtistId.Value);
                    if (approvedArtist != null && approvedArtist.Status == "Active")
                    {
                        qualifiedArtists = new List<NailArtist> { approvedArtist };
                    }
                    else
                    {
                        qualifiedArtists = new List<NailArtist>();
                    }
                }
                else
                {
                    qualifiedArtists = new List<NailArtist>();
                }
            }
            else
            {
                var variantIds = _unitOfWork.NailVariantRepository.GetDistinctVariantIdsAsync(bookingItems);
                if (variantIds.Any())
                {
                    qualifiedArtists = await _unitOfWork.NailArtistRepository.GetSuggestedArtistsAsync(request.SalonId, variantIds);
                }
                else
                {
                    var activeArtists = await _unitOfWork.NailArtistRepository.GetNailArtistsBySalonIdAsync(request.SalonId);
                    qualifiedArtists = activeArtists.Where(x => x.Status == "Active");
                }
            }

            var availableArtists = new List<NailArtist>();
            var targetEndTime = request.StartTime.Add(TimeSpan.FromMinutes(totalDuration));

            var dayOfWeek = (int)localDate.DayOfWeek;

            var salon = await _unitOfWork.SalonRepository.GetSalonWithOperatingHoursAsync(request.SalonId);
            var operatingHours = salon?.OperatingHours?.Where(x => x.DayOfWeek == dayOfWeek).ToList() ?? new List<SalonOperatingHour>();
            if (!operatingHours.IsWithinOperatingHours(request.StartTime, targetEndTime))
            {
                return new ApiErrorResult<SuggestedArtistResponseDTO>("Thời gian đặt lịch không nằm trong giờ hoạt động của Salon.");
            }

            foreach (var artist in qualifiedArtists)
            {
                var salonId = artist.Account.SalonId ?? Guid.Empty;

                var artistBreaks = await _unitOfWork.NailArtistBreakRepository.GetApprovedBreaksByArtistAndDateAsync(artist.NailArtistId, request.BookingDate);
                bool overlapsBreak = artistBreaks.Any(x => request.StartTime < x.EndTime && targetEndTime > x.StartTime);
                if (overlapsBreak)
                {
                    continue;
                }
                var schedule = await _unitOfWork.ScheduleRepository.GetScheduleByArtistAndDateAsync(artist.NailArtistId, request.BookingDate);
                if (schedule == null) continue;

                if (request.StartTime < schedule.ShiftStart || targetEndTime > schedule.ShiftEnd) continue;

                var mockProcs = await _bookingSchedulingService.GenerateMockBookingProceduresAsync(request.BookingItems.ToList(), request.SalonId);
                var timeline = _bookingSchedulingService.BuildProcedureTimeline(mockProcs, request.StartTime);
                var isConflict = await _bookingSchedulingService.HasCapacityConflictAsync(
                artist.NailArtistId, request.BookingDate, timeline, artist.ConcurrentCapacity);
                if (isConflict) continue;

                availableArtists.Add(artist);
            }

            if (!availableArtists.Any())
            {
                return new ApiErrorResult<SuggestedArtistResponseDTO>("Không có thợ nào có đủ trình độ và thời gian trống trong khung giờ này.");
            }

            NailArtist? bestArtist = null;
            int minBookings = int.MaxValue;

            foreach (var artist in availableArtists)
            {
                var bookingsOnDay = await _unitOfWork.BookingRepository.GetBookingsByArtistAndDateAsync(artist.NailArtistId, request.BookingDate);
                int count = bookingsOnDay.Count();
                if (count < minBookings)
                {
                    minBookings = count;
                    bestArtist = artist;
                }
            }

            if (bestArtist == null)
            {
                return new ApiErrorResult<SuggestedArtistResponseDTO>("Không thể tự động phân bổ thợ.");
            }

            var response = _mapper.Map<SuggestedArtistResponseDTO>(bestArtist);
            return new ApiSuccessResult<SuggestedArtistResponseDTO>(response, "Đã chọn thợ ngẫu nhiên tối ưu nhất thành công.");
        }
        public async Task<ApiResult<ArtistAvailabilityResponseDTO>> GetArtistAvailableSlotAsync(GetArtistAvailableSlotsRequestDTO request)

        {
            var artist = await _unitOfWork.NailArtistRepository.GetNailArtistWithProfileAsync(request.NailArtistId);
            if (artist == null)
            {
                return new ApiErrorResult<ArtistAvailabilityResponseDTO>("Không tìm thấy thợ nail.");
            }

            var schedule = await _unitOfWork.ScheduleRepository.GetScheduleByArtistAndDateAsync(request.NailArtistId, request.BookingDate);
            if (schedule == null)
            {
                return new ApiErrorResult<ArtistAvailabilityResponseDTO>("Thợ nail không có lịch làm việc trong ngày này.");
            }
            var salonId = artist.Account?.SalonId ?? Guid.Empty;
            var localDate = (request.BookingDate.Kind == DateTimeKind.Utc ? request.BookingDate.AddHours(7) : request.BookingDate).Date;
            var isOffDay = await _unitOfWork.SalonOffDateRepository.ExistsAsync(x =>
                                                            x.SalonId == salonId
                                                            && x.StartDate.Date <= localDate
                                                            && x.EndDate.Date >= localDate);
            if (isOffDay)
            {
                return new ApiSuccessResult<ArtistAvailabilityResponseDTO>(new ArtistAvailabilityResponseDTO
                {
                    NailArtistId = artist.NailArtistId,
                    ArtistName = $"{artist.Account.FirstName} {artist.Account.LastName}",
                    AvailabilityStatus = "Off",
                    TimeSlots = new List<TimeSlotResponseDTO>()
                }, "Hôm nay là ngày nghỉ của Salon.");
            }
            var mockProcedures = new List<BookingProcedure>();
            int tempStepOrder = 1;
            if (request.BookingItems != null)
            {
                foreach (var item in request.BookingItems)
                {
                    if (item.NailVariantId.HasValue)
                    {
                        var activeNailProcedures = await _unitOfWork.NailProcedureRepository.GetActiveProceduresByVariantIdAsync(item.NailVariantId.Value);
                        foreach (var x in activeNailProcedures)
                        {
                            mockProcedures.Add(new BookingProcedure
                            {
                                BookingProcedureId = Guid.NewGuid(),
                                StepOrder = tempStepOrder++,
                                Duration = x.Procedure.Duration ?? 0,
                                ActiveDuration = x.Procedure.ActiveDuration,
                                PassiveDuration = x.Procedure.PassiveDuration,
                                CanOverlap = x.Procedure.PassiveDuration >= 4 && x.Procedure.CanOverlap,
                                TransitionBuffer = x.Procedure.TransitionBuffer > 0 ? x.Procedure.TransitionBuffer : 1
                            });
                        }
                    }
                    // 3.2. Nếu là dịch vụ lẻ
                    if (item.ServiceId.HasValue)
                    {
                        var service = await _unitOfWork.ServicesRepository.GetByIdAsync(item.ServiceId.Value);
                        if (service != null)
                        {
                            mockProcedures.Add(new BookingProcedure
                            {
                                BookingProcedureId = Guid.NewGuid(),
                                StepOrder = tempStepOrder++,
                                Duration = service.Duration,
                                ActiveDuration = service.Duration, // Mặc định dịch vụ lẻ là thợ bận toàn bộ thời gian
                                PassiveDuration = 0,
                                CanOverlap = false
                            });
                        }
                    }
                    if (item.CustomerNailRequestId.HasValue)
                    {
                        // Tìm yêu cầu đã được duyệt báo giá để lấy thời gian thi công thực tế tại chi nhánh
                        var customNailRequest = await _unitOfWork.CustomerNailRequestRepository.GetByIdAsync(item.CustomerNailRequestId.Value);
                        int duration = 60; // Thời gian mặc định

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
                            StepOrder = tempStepOrder++,
                            Duration = duration,
                            ActiveDuration = duration, // Mặc định custom nail thợ bận toàn bộ thời gian
                            PassiveDuration = 0,
                            CanOverlap = false
                        });
                    }
                }
            }

            if (!mockProcedures.Any())
            {
                // Mặc định giả lập 1 slot 15 phút bận toàn bộ để hiển thị chính xác các slot thợ bận/giữ chỗ
                mockProcedures.Add(new BookingProcedure
                {
                    BookingProcedureId = Guid.NewGuid(),
                    StepOrder = tempStepOrder++,
                    Duration = 15,
                    ActiveDuration = 15,
                    PassiveDuration = 0,
                    CanOverlap = false
                });
            }
            var existingBusySegments = await _unitOfWork.BookingProcedureRepository
                                                        .GetArtistBusySegmentsByDateAsync(request.NailArtistId, request.BookingDate);


            var dayOfWeek = (int)localDate.DayOfWeek;
            var salon = await _unitOfWork.SalonRepository.GetSalonWithOperatingHoursAsync(salonId);
            var operatingHours = salon?.OperatingHours?.Where(x => x.DayOfWeek == dayOfWeek).ToList() ?? new List<SalonOperatingHour>();

            var artistBreaks = await _unitOfWork.NailArtistBreakRepository.GetApprovedBreaksByArtistAndDateAsync(request.NailArtistId, request.BookingDate);

            var timeSlots = new List<TimeSlotResponseDTO>();
            var candidateStart = schedule.ShiftStart;
            var interval = TimeSpan.FromMinutes(15);
            int totalDuration = mockProcedures.Sum(x => x.Duration);
            while (candidateStart.Add(TimeSpan.FromMinutes(totalDuration)) <= schedule.ShiftEnd)
            {
                var targetEndTime = candidateStart.Add(TimeSpan.FromMinutes(totalDuration));

                bool isWithinSalonHours = operatingHours.IsWithinOperatingHours(candidateStart, targetEndTime);

                bool overlapsBreak = artistBreaks.Any(x => candidateStart < x.EndTime && targetEndTime > x.StartTime);

                bool isAvailable = false;
                bool isHeld = false;
                if (isWithinSalonHours && !overlapsBreak)
                {
                    // A. Tạo timeline giả lập xuất phát từ candidateStart
                    var timeline = _bookingSchedulingService.BuildProcedureTimeline(mockProcedures, candidateStart);

                    // B. Check conflict trên các khoảng ActiveDuration > 0 của thợ
                    var isConflict = await _bookingSchedulingService.HasCapacityConflictAsync(
                        request.NailArtistId,
                        request.BookingDate,
                        timeline,
                        artist.ConcurrentCapacity
                    );

                    isAvailable = !isConflict;
                    // C. Nếu không conflict lịch trong DB, kiểm tra tiếp trạng thái giữ chỗ tạm thời (Redis Hold)
                    if (isAvailable)
                    {
                        isHeld = await _slotHoldService.IsSlotHeldAsync(
                            request.NailArtistId,
                            request.BookingDate,
                            candidateStart,
                            targetEndTime
                        );
                    }
                }
                timeSlots.Add(new TimeSlotResponseDTO
                {
                    StartTime = candidateStart,
                    EndTime = candidateStart.Add(interval),
                    IsAvailable = isAvailable && !isHeld,
                    IsHeld = isHeld
                });
                candidateStart = candidateStart.Add(interval);
            }
            // 6. Map dữ liệu busySlots thực tế của thợ để hiển thị trên UI
            var busySlots = existingBusySegments
                .Select(x => new BusyTimeSlotResponseDto
                {
                    StartTime = x.ArtistBusyStart,
                    EndTime = x.ArtistBusyEnd
                })
                .OrderBy(x => x.StartTime)
                .ToList();

            var response = new ArtistAvailabilityResponseDTO
            {
                NailArtistId = artist.NailArtistId,
                ArtistName = $"{artist.Account.FirstName} {artist.Account.LastName}",
                AvatarUrl = artist.Account.AvatarUrl ?? "",
                AvailabilityStatus = "Working",
                ShiftStart = schedule.ShiftStart,
                ShiftEnd = schedule.ShiftEnd,
                BusySlots = busySlots,
                TimeSlots = timeSlots
            };
            return new ApiSuccessResult<ArtistAvailabilityResponseDTO>(response, "Lấy thông tin slot làm việc thành công.");
        }
        public async Task<ApiResult<List<SuggestedArtistResponseDTO>>> GetAvailableArtistsForBookingAsync(Guid bookingId)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(bookingId);
            if (booking == null)
            {
                return new ApiErrorResult<List<SuggestedArtistResponseDTO>>("Không tìm thấy thông tin đặt lịch.");
            }
            var bookingItems = booking.BookingItems.ToList();
            var targetEndTime = booking.StartTime.Add(TimeSpan.FromMinutes(booking.TotalDuration));
            var procedures = await _unitOfWork.BookingProcedureRepository
                                              .GetProceduresByBookingIdAsync(bookingId);
            if (!procedures.Any())
            {
                var itemRequests = bookingItems.Select(x => new BookingItemRequestDTO
                {
                    ServiceId = x.ServiceId,
                    NailVariantId = x.NailVariantId,
                    CustomerNailRequestId = x.CustomerNailRequestId,
                    Quantity = x.Quantity
                }).ToList();

                procedures = await _bookingSchedulingService.GenerateMockBookingProceduresAsync(itemRequests, booking.SalonId);
            }

            var timeline = _bookingSchedulingService.BuildProcedureTimeline(
                procedures, booking.StartTime);

            // Custom: Chỉ cho phép thợ duyệt mẫu custom nhận làm
            var customNailItem = bookingItems.FirstOrDefault(x => x.CustomerNailRequestId.HasValue);
            if (customNailItem != null)
            {
                var customNailRequest = await _unitOfWork.CustomerNailRequestRepository.GetByIdAsync(customNailItem.CustomerNailRequestId!.Value);
                if (customNailRequest != null &&
                    customNailRequest.SalonId == booking.SalonId &&
                    (customNailRequest.Status == CustomerNailStatus.Approved || customNailRequest.Status == CustomerNailStatus.Quoted))
                {
                    if (customNailRequest != null && customNailRequest.ApprovedArtistId.HasValue)
                    {
                        var approvedArtist = await _unitOfWork.NailArtistRepository.GetNailArtistWithProfileAsync(customNailRequest.ApprovedArtistId.Value);
                        if (approvedArtist != null && approvedArtist.Status == "Active")
                        {
                            var schedule = await _unitOfWork.ScheduleRepository.GetScheduleByArtistAndDateAsync(approvedArtist.NailArtistId, booking.BookingDate);
                            if (schedule != null && booking.StartTime >= schedule.ShiftStart && targetEndTime <= schedule.ShiftEnd)
                            {
                                var isConflict = await _bookingSchedulingService.HasCapacityConflictAsync(
                                  approvedArtist.NailArtistId,
                                  booking.BookingDate,
                                  timeline,
                                  approvedArtist.ConcurrentCapacity,
                                  bookingId);
                                if (!isConflict)
                                {
                                    var singleArtistDto = _mapper.Map<List<SuggestedArtistResponseDTO>>(new List<NailArtist> { approvedArtist });
                                    return new ApiSuccessResult<List<SuggestedArtistResponseDTO>>(singleArtistDto, "Lấy thợ rảnh thành công.");
                                }
                            }
                            // Nếu thợ đã duyệt bận hoặc không có ca làm, trả về danh sách trống để Lễ tân báo khách hàng
                            return new ApiSuccessResult<List<SuggestedArtistResponseDTO>>(new List<SuggestedArtistResponseDTO>(), "Thợ đã thẩm định mẫu custom hiện đang bận hoặc không có lịch làm việc hôm nay.");
                        }
                    }
                }
            }
            var artists = await _unitOfWork.NailArtistRepository.GetNailArtistsBySalonIdAsync(booking.SalonId);
            var activeArtists = artists.Where(x => x.Status == "Active").ToList();
            var variantIds = bookingItems.Where(x => x.NailVariantId.HasValue).Select(x => x.NailVariantId!.Value).Distinct().ToList();
            List<NailArtist> qualifiedArtists;
            if (variantIds.Any())
            {
                qualifiedArtists = await _unitOfWork.NailArtistRepository.GetSuggestedArtistsAsync(booking.SalonId, variantIds);
            }
            else
            {
                qualifiedArtists = activeArtists;
            }
            var availableArtists = new List<NailArtist>();
            foreach (var artist in qualifiedArtists)
            {
                var schedule = await _unitOfWork.ScheduleRepository.GetScheduleByArtistAndDateAsync(artist.NailArtistId, booking.BookingDate);
                if (schedule == null) continue;
                if (booking.StartTime < schedule.ShiftStart || targetEndTime > schedule.ShiftEnd) continue;
                if (!await _skillMatchingService.HasRequiredSkillsAsync(artist, booking, booking.NailArtistId)) continue;
                var isConflict = await _bookingSchedulingService.HasCapacityConflictAsync(
                                         artist.NailArtistId,
                                         booking.BookingDate,
                                         timeline,
                                         artist.ConcurrentCapacity,
                                         bookingId);
                if (isConflict) continue;
                availableArtists.Add(artist);
            }
            var response = _mapper.Map<List<SuggestedArtistResponseDTO>>(availableArtists);
            return new ApiSuccessResult<List<SuggestedArtistResponseDTO>>(response, "Lấy danh sách thợ rảnh cho đơn đặt lịch thành công.");
        }
        public async Task<ApiResult<BookingResponseDTO>> ReceptionistAssignArtistAsync(Guid bookingId, AssignArtistRequestDTO request, Guid actorId)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(bookingId, trackChanges: true);
            if (booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Không tìm thấy thông tin đặt lịch.");
            }

            if (booking.Status != BookingStatus.Pending
                && booking.Status != BookingStatus.Approved
                && booking.Status != BookingStatus.CheckedIn
                && booking.Status != BookingStatus.InProgress)
            {
                return new ApiErrorResult<BookingResponseDTO>($"Không thể chỉ định thợ cho lịch hẹn ở trạng thái '{booking.Status}'.");
            }
            var artist = await _unitOfWork.NailArtistRepository.GetNailArtistWithProfileAsync(request.StaffArtistId);
            if (artist == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Không tìm thấy thông tin thợ.");
            }
            var procedures = await _unitOfWork.BookingProcedureRepository.GetProceduresByBookingIdAsync(bookingId, trackChanges: true);
            if (procedures.Any())
            {
                var timeline = _bookingSchedulingService.BuildProcedureTimeline(procedures, booking.StartTime);
                var isConflict = await _bookingSchedulingService.HasCapacityConflictAsync(
                    request.StaffArtistId,
                    booking.BookingDate,
                    timeline,
                    artist.ConcurrentCapacity,
                    bookingId
                );
                if (isConflict)
                {
                    return new ApiErrorResult<BookingResponseDTO>($"Thợ {artist.Account.FirstName} {artist.Account.LastName} đã bị trùng hoặc quá tải lịch làm việc trong khung giờ này.");
                }

                // Cập nhật gán thợ và timeline cho các bước con
                foreach (var segment in timeline)
                {
                    var proc = procedures.First(x => x.BookingProcedureId == segment.BookingProcedureId);
                    /*
                    proc.EstimatedStartTime = segment.StartTime;
                    proc.EstimatedEndTime = segment.EndTime;
                    if (proc.ActiveDuration > 0 && proc.IsMainStep)
                    {
                        proc.AssignedArtistId = request.StaffArtistId;
                    }
                    */
                    if (proc.Status == BookingProcedureStatus.Pending)
                    {
                        proc.EstimatedStartTime = segment.StartTime;
                        proc.EstimatedEndTime = segment.EndTime;
                        if (proc.IsMainStep)
                        {
                            proc.AssignedArtistId = request.StaffArtistId;
                        }
                        _unitOfWork.BookingProcedureRepository.Update(proc);
                    }
                }
            }
            // Cập nhật thợ nail
            booking.ReceptionistAssignArtist(request.StaffArtistId, $"{artist.Account.FirstName} {artist.Account.LastName}", actorId);

            _unitOfWork.BookingRepository.Update(booking);
            await _unitOfWork.SaveChangesAsync();

            // Bắn SignalR cập nhật
            await _notificationService.SendNotificationToUserAsync(
                booking.CustomerId.ToString(),
                "ArtistReassigned",
                new { BookingId = bookingId, NewArtistName = $"{artist.Account?.FirstName} {artist.Account?.LastName}" }
            );
            var savedBooking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(booking.BookingId);
            var response = _mapper.Map<BookingResponseDTO>(savedBooking);
            return new ApiSuccessResult<BookingResponseDTO>(response, "Tiếp tân chỉ định thợ nail thành công.");
        }
        public async Task<ApiResult<BookingResponseDTO>> AssignChairAsync(Guid bookingId, Guid chairId, Guid actorId)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(bookingId, trackChanges: true);
            if (booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Không tìm thấy thông tin đặt lịch.");
            }

            if (booking.Status == BookingStatus.Cancelled || booking.Status == BookingStatus.Rejected)
            {
                return new ApiErrorResult<BookingResponseDTO>($"Không thể gán ghế cho lịch hẹn đã bị hủy hoặc từ chối.");
            }

            var chair = await _unitOfWork.ChairRepository.GetByIdAsync(chairId);
            if (chair == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Không tìm thấy thông tin ghế.");
            }

            if (chair.SalonId != booking.SalonId)
            {
                return new ApiErrorResult<BookingResponseDTO>("Ghế được chọn không thuộc chi nhánh của lịch hẹn này.");
            }

            if (chair.Status != "Active")
            {
                return new ApiErrorResult<BookingResponseDTO>($"Ghế '{chair.ChairName}' hiện không khả dụng (Trạng thái: {chair.Status}).");
            }

            // Check if the chair is occupied during this time
            var requestedStart = booking.StartTime;
            var requestedEnd = booking.StartTime.Add(TimeSpan.FromMinutes(booking.TotalDuration));

            var overlappingBookings = await _unitOfWork.BookingRepository.GetBookingsByChairAndDateAsync(chairId, booking.BookingDate);

            foreach (var ob in overlappingBookings)
            {
                if (ob.BookingId == bookingId) continue;

                var obStart = ob.StartTime;
                var obEnd = ob.StartTime.Add(TimeSpan.FromMinutes(ob.TotalDuration));

                if (obStart < requestedEnd && requestedStart < obEnd)
                {
                    return new ApiErrorResult<BookingResponseDTO>($"Ghế '{chair.ChairName}' đã được sử dụng bởi một lịch hẹn khác trong khung giờ {obStart} - {obEnd}.");
                }
            }

            // Assign chair
            booking.AssignChair(chairId, chair.ChairName, actorId);
            _unitOfWork.BookingRepository.Update(booking);
            await _unitOfWork.SaveChangesAsync();

            // Refresh booking detail for response
            var refreshedBooking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(bookingId);
            var response = _mapper.Map<BookingResponseDTO>(refreshedBooking);
            return new ApiSuccessResult<BookingResponseDTO>(response, $"Phân bổ ghế '{chair.ChairName}' cho lịch hẹn thành công.");
        }
        public async Task<ApiResult<CustomerWaitEtaResponseDTO>> GetPreBookedCustomerWaitTimeEtaAndCompensateAsync(Guid bookingId)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(bookingId);
            if (booking == null)
            {
                return new ApiErrorResult<CustomerWaitEtaResponseDTO>("Không tìm thấy lịch hẹn.");
            }
            if (!booking.NailArtistId.HasValue)
            {
                return new ApiErrorResult<CustomerWaitEtaResponseDTO>("Lịch hẹn chưa được gán thợ.");
            }

            var artistId = booking.NailArtistId.Value;
            var activeProcs = await _unitOfWork.BookingProcedureRepository.GetActiveProceduresByArtistIdAsync(artistId);
            var inProgress = activeProcs.FirstOrDefault(x => x.Status == BookingProcedureStatus.InProgress);

            var localNow = DateTime.UtcNow.AddHours(7);
            int estimatedWaitMinutes = 0;
            if (inProgress != null && inProgress.ActualStartTime.HasValue)
            {
                var elapsed = (localNow - inProgress.ActualStartTime.Value).TotalMinutes;
                estimatedWaitMinutes = Math.Max(1, (int)(inProgress.Duration - elapsed));
            }

            bool compensationApplied = false;
            string compType = string.Empty;
            string displayMsg = $"Thợ của bạn đang hoàn thiện bước cuối, dự kiến phục vụ sau {estimatedWaitMinutes} phút.";

            var localBookingDate = (booking.BookingDate.Kind == DateTimeKind.Utc ? booking.BookingDate.AddHours(7) : booking.BookingDate).Date;
            var scheduledStartTime = localBookingDate + booking.StartTime;
            var delayFromStartTime = (localNow - scheduledStartTime).TotalMinutes;
            if (delayFromStartTime > 10)
            {
                // HƯỚNG DẪN LUỒNG XỬ LÝ ĐỀN BÙ KHI KHÁCH HÀNG CHỜ QUÁ 10 PHÚT (BR-01.4):
                //  Mục tiêu: Đền bù tự động cho khách hàng khi thợ bị trễ ca > 10 phút so với giờ hẹn (`StartTime`).
                // TuePDG
            }

            // HƯỚNG DẪN LUỒNG XỬ LÝ ĐỀN BÙ KHI KHÁCH HÀNG CHỜ QUÁ 10 PHÚT (BR-01.4):
            // Mục tiêu: Đền bù tự động cho khách hàng khi thợ bị trễ ca > 10 phút so với giờ hẹn (`StartTime`).
            // TuePDG
            /*
            var existingHistory = await _unitOfWork.BookingHistoryRepository.ExistsAsync(h =>
                      h.BookingId == bookingId && h.EventType == "WaitTimeCompensationApplied");
                    */
            var result = _mapper.Map<CustomerWaitEtaResponseDTO>(booking);
            result.EstimatedWaitMinutes = estimatedWaitMinutes;
            result.StatusMessage = "Tính toán ETA thời gian chờ thành công.";
            result.DisplayMessage = displayMsg;
            return new ApiSuccessResult<CustomerWaitEtaResponseDTO>(result);
        }

        public async Task<ApiResult<SalonAvailabilityResponseDTO>> GetSalonAvailableSlotsAsync(GetSalonAvailableSlotsRequestDTO request)
        {
            if (request.BookingItems == null || !request.BookingItems.Any())
            {
                return new ApiErrorResult<SalonAvailabilityResponseDTO>("Vui lòng chọn dịch vụ hoặc mẫu nail trước khi xem khung giờ trống.");
            }
            var bookingItems = _mapper.Map<List<BookingItem>>(request.BookingItems);
            if (bookingItems.Any(item =>
                                        !item.NailVariantId.HasValue
                                        && !item.ServiceId.HasValue
                                        && !item.CustomerNailRequestId.HasValue))
            {
                return new ApiErrorResult<SalonAvailabilityResponseDTO>("Mỗi mục đặt lịch phải chứa ít nhất một dịch vụ, một mẫu nail hoặc một mẫu custom.");
            }

            var salon = await _unitOfWork.SalonRepository.GetSalonWithOperatingHoursAsync(request.SalonId);
            if (salon == null)
            {
                return new ApiErrorResult<SalonAvailabilityResponseDTO>("Không tìm thấy thông tin salon.");
            }

            var localDate = (request.BookingDate.Kind == DateTimeKind.Utc ? request.BookingDate.AddHours(7) : request.BookingDate).Date;
            var isOffDay = await _unitOfWork.SalonOffDateRepository.ExistsAsync(x =>
                                                                                     x.SalonId == request.SalonId
                                                                                     && x.StartDate.Date <= localDate
                                                                                     && x.EndDate.Date >= localDate);
            if (isOffDay)
            {
                return new ApiSuccessResult<SalonAvailabilityResponseDTO>(new SalonAvailabilityResponseDTO
                {
                    SalonId = request.SalonId,
                    TimeSlots = new List<SalonTimeSlotResponseDTO>()
                }, "Salon đóng cửa nghỉ lễ vào ngày này.");
            }

            // Gio hoat dong cua salon trong ngay
            var dayOfWeek = (int)localDate.DayOfWeek;
            var operatingHours = salon.OperatingHours?.Where(x => x.DayOfWeek == dayOfWeek).ToList() ?? new List<SalonOperatingHour>();
            if (!operatingHours.Any() || operatingHours.Any(x => x.IsClosed))
            {
                return new ApiSuccessResult<SalonAvailabilityResponseDTO>(new SalonAvailabilityResponseDTO
                {
                    SalonId = request.SalonId,
                    TimeSlots = new List<SalonTimeSlotResponseDTO>()
                }, "Salon không hoạt động vào ngày này.");
            }

            var salonOpenTime = operatingHours.Min(x => x.OpenTime);
            var salonCloseTime = operatingHours.Max(x => x.CloseTime);

            // Giả lập procedures để tính tổng thời lượng chính xác
            var mockProcedures = await _bookingSchedulingService.GenerateMockBookingProceduresAsync(request.BookingItems.ToList(), request.SalonId);
            if (!mockProcedures.Any())
            {
                return new ApiErrorResult<SalonAvailabilityResponseDTO>("Không xác định được thời lượng dịch vụ từ các mục đã chọn.");
            }
            int totalDuration = mockProcedures.Sum(x => x.Duration);

            // Lọc danh sách thợ đủ điều kiện
            IEnumerable<NailArtist> qualifiedArtists;
            var customNailItem = bookingItems.FirstOrDefault(x => x.CustomerNailRequestId.HasValue);
            if (customNailItem != null)
            {
                // Mẫu custom: chỉ thợ đã duyệt báo giá mẫu này được nhận
                var customNailRequest = await _unitOfWork.CustomerNailRequestRepository.GetByIdAsync(customNailItem.CustomerNailRequestId!.Value);
                if (customNailRequest != null
                   && customNailRequest.SalonId == request.SalonId
                   && (
                        customNailRequest.Status == CustomerNailStatus.Approved
                        || customNailRequest.Status == CustomerNailStatus.Quoted
                      ) && customNailRequest.ApprovedArtistId.HasValue)
                {
                    var approvedArtist = await _unitOfWork.NailArtistRepository.GetNailArtistWithProfileAsync(customNailRequest.ApprovedArtistId.Value);
                    qualifiedArtists = (approvedArtist != null && approvedArtist.Status == "Active") ? new List<NailArtist> { approvedArtist }
                    : new List<NailArtist>();
                }
                else
                {
                    qualifiedArtists = new List<NailArtist>();
                }
            }
            else
            {
                var variantIds = _unitOfWork.NailVariantRepository.GetDistinctVariantIdsAsync(bookingItems);
                if (variantIds.Any())
                {
                    qualifiedArtists = await _unitOfWork.NailArtistRepository.GetSuggestedArtistsAsync(request.SalonId, variantIds);
                }
                else
                {
                    var activeArtists = await _unitOfWork.NailArtistRepository.GetNailArtistsBySalonIdAsync(request.SalonId);
                    qualifiedArtists = activeArtists.Where(x => x.Status == "Active");
                }
            }
            // Pre-load dữ liệu 1 lần cho mỗi thợ (tránh N×M query trong vòng lặp slot)
            var artistContexts = new List<(NailArtist Artist, Schedule Schedule, List<NailArtistBreak> Breaks, List<ProcedureScheduleSegment> BusySegments, List<(TimeSpan Start, TimeSpan End)> HoldRanges)>();
            foreach (var artist in qualifiedArtists)
            {
                var schedule = await _unitOfWork.ScheduleRepository.GetScheduleByArtistAndDateAsync(artist.NailArtistId, request.BookingDate);
                if (schedule == null)
                {
                    continue; // Thợ không có ca làm trong ngày này
                }
                var breaks = await _unitOfWork.NailArtistBreakRepository.GetApprovedBreaksByArtistAndDateAsync(artist.NailArtistId, request.BookingDate);
                var busySegments = await _unitOfWork.BookingProcedureRepository.GetArtistBusySegmentsByDateAsync(artist.NailArtistId, request.BookingDate);
                var holdRanges = await _slotHoldService.GetActiveHoldRangesAsync(artist.NailArtistId, request.BookingDate);

                artistContexts.Add((artist, schedule, breaks, busySegments, holdRanges));
            }

            // Duyệt từng slot 15 phút trong giờ hoạt động salon, đếm số thợ rảnh mỗi slot
            var timeSlots = new List<SalonTimeSlotResponseDTO>();
            var interval = TimeSpan.FromMinutes(15);
            var candidateStart = salonOpenTime;
            while (candidateStart.Add(TimeSpan.FromMinutes(totalDuration)) <= salonCloseTime)
            {
                var targetEndTime = candidateStart.Add(TimeSpan.FromMinutes(totalDuration));
                int availableCount = 0;

                if (operatingHours.IsWithinOperatingHours(candidateStart, targetEndTime))
                {
                    // Timeline giả lập tại slot này dùng chung cho mọi thợ
                    var timeLine = _bookingSchedulingService.BuildProcedureTimeline(mockProcedures, candidateStart);

                    foreach (var ctx in artistContexts)
                    {
                        // Slot phải nằm trọn trong ca làm của thợ
                        if (candidateStart < ctx.Schedule.ShiftStart
                           || targetEndTime > ctx.Schedule.ShiftEnd)
                        {
                            continue;
                        }
                        // Không trùng giờ nghỉ đã duyệt của thợ
                        if (ctx.Breaks.Any(x => candidateStart < x.EndTime && targetEndTime > x.StartTime))
                        {
                            continue;
                        }
                        // Không conflict với lịch bận hiện có (check in-memory trên dữ liệu pre-load)
                        var conflict = _bookingSchedulingService.HasCapacityConflictInMemory(ctx.Artist.NailArtistId, ctx.BusySegments, timeLine, ctx.Artist.ConcurrentCapacity);
                        if (conflict)
                        {
                            continue;
                        }

                        // Không bị giữ chỗ tạm thời (Redis Hold)
                        if (ctx.HoldRanges.Any(x => x.Start < targetEndTime && x.End > candidateStart))
                        {
                            continue;
                        }

                        availableCount++;
                    }
                }
                timeSlots.Add(new SalonTimeSlotResponseDTO
                {
                    StartTime = candidateStart,
                    EndTime = candidateStart.Add(interval),
                    IsAvailable = availableCount > 0,

                });
                candidateStart = candidateStart.Add(interval);
            }
            var response = new SalonAvailabilityResponseDTO
            {
                SalonId = request.SalonId,
                SalonOpenTime = salonOpenTime,
                SalonCloseTime = salonCloseTime,
                TimeSlots = timeSlots
            };
            return new ApiSuccessResult<SalonAvailabilityResponseDTO>(response, "Lấy khung giờ trống của salon thành công.");
        }

        public async Task<ApiResult<TransferPreviewResponseDTO>> PreviewTransferSalonAsync(Guid bookingId, Guid targetSalonId, Guid actorId)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(bookingId);
            if(booking == null)
            {
                return new ApiErrorResult<TransferPreviewResponseDTO>("Không tìm thấy thông tin đặt lịch.");
            }
            if(booking.Status != BookingStatus.Pending && booking.Status != BookingStatus.Approved)
            {
                return new ApiErrorResult<TransferPreviewResponseDTO>($"Chỉ cho phép chuyển khi Pending hoặc Approved. Trạng thái hiện tại: '{booking.Status}'.");

            }
            if(booking.SalonId == targetSalonId)
            {
                return new ApiErrorResult<TransferPreviewResponseDTO>("Chi nhánh đích trùng với chi nhánh hiện tại.");
            }
            var targetSalon = await _unitOfWork.SalonRepository.GetSalonWithOperatingHoursAsync(targetSalonId);
            if(targetSalon == null)
            {
                return new ApiErrorResult<TransferPreviewResponseDTO>("Không tìm thấy chi nhánh đích.");
            }

            var localDate = (booking.BookingDate.Kind == DateTimeKind.Utc
                             ? booking.BookingDate.AddHours(7)
                             : booking.BookingDate).Date;

            var isOffDay = await _unitOfWork.SalonOffDateRepository.ExistsAsync(x => x.SalonId == targetSalonId
                                                                                     && x.StartDate.Date <= localDate
                                                                                     && x.EndDate.Date >= localDate);
            if (isOffDay)
            {
                return new ApiErrorResult<TransferPreviewResponseDTO>("Chi nhánh đích đang trong ngày nghỉ lễ.");
            }
            var dayOfWeek = (int)localDate.DayOfWeek;
            var operatingHours = targetSalon.OperatingHours?.Where(x => x.DayOfWeek == dayOfWeek).ToList() ?? new List<SalonOperatingHour>();
            var targetEnd = booking.StartTime.Add(TimeSpan.FromMinutes(booking.TotalDuration));

            if(!operatingHours.IsWithinOperatingHours(booking.StartTime, targetEnd))
            {
                return new ApiErrorResult<TransferPreviewResponseDTO>("Khung giờ đặt lịch không nằm trong giờ hoạt động của chi nhánh đích.");
            }
            var procedures = await _unitOfWork.BookingProcedureRepository.GetProceduresByBookingIdAsync(bookingId);
            var timeline = _bookingSchedulingService.BuildProcedureTimeline(procedures, booking.StartTime);
            var allArtists = await _unitOfWork.NailArtistRepository.GetNailArtistsBySalonIdAsync(targetSalonId);
            var availableArtists = new List<NailArtist>();
            foreach (var artist in allArtists.Where(x => x.Status == "Active"))
            {
                var schedule = await _unitOfWork.ScheduleRepository.GetScheduleByArtistAndDateAsync(artist.NailArtistId, booking.BookingDate);
                if(schedule == null)
                {
                    continue;
                }
                if(booking.StartTime < schedule.ShiftStart || targetEnd > schedule.ShiftEnd)
                {
                    continue;
                }
                var requiredSkill = await _skillMatchingService.HasRequiredSkillsAsync(artist, booking, null);
                if(!requiredSkill)
                {
                    continue;
                }
                var hasConflict = await _bookingSchedulingService.HasCapacityConflictAsync(artist.NailArtistId, booking.BookingDate, timeline, artist.ConcurrentCapacity);
                if(!hasConflict)
                {
                    availableArtists.Add(artist);
                }
            }
            var originalSalon = await _unitOfWork.SalonRepository.GetSalonWithOperatingHoursAsync(booking.SalonId);

            var response = new TransferPreviewResponseDTO
            {
                BookingId = bookingId,
                OriginalSalonId = booking.SalonId,
                OriginalSalonName = originalSalon?.Name ?? booking.SalonId.ToString(),
                TargetSalonId = targetSalonId,
                TargetSalonName = targetSalon.Name,
                TotalPrice = booking.TotalPrice ?? 0,
                AvailableArtists = _mapper.Map<List<SuggestedArtistResponseDTO>>(availableArtists),
                CanTransfer = true,
                WarningMessage = !availableArtists.Any()
                                                         ? "Không có thợ rảnh tại chi nhánh đích trong khung giờ này. Vẫn có thể chuyển và gán thợ sau."
                                                         : null
            };
            return new ApiSuccessResult<TransferPreviewResponseDTO>(response, "Xem trước chuyển chi nhánh thành công.");
        }

        public async Task<ApiResult<BookingResponseDTO>> TransferSalonAsync(Guid bookingId, TransferSalonRequestDTO request, Guid actorId)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(bookingId, trackChanges: true);
            if(booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Không tìm thấy thông tin đặt lịch.");
            }
            if(booking.Status !=  BookingStatus.Pending && booking.Status != BookingStatus.Approved)
            {
                return new ApiErrorResult<BookingResponseDTO>($"Chỉ cho phép chuyển khi Pending hoặc Approved. Trạng thái hiện tại: '{booking.Status}'.");
            }
            if(booking.SalonId == request.TargetSalonId)
            {
                return new ApiErrorResult<BookingResponseDTO>("Chi nhánh đích trùng với chi nhánh hiện tại.");
            }
            var targetSalon = await _unitOfWork.SalonRepository.GetSalonWithOperatingHoursAsync(request.TargetSalonId);
            if(targetSalon == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Không tìm thấy chi nhánh đích.");
            }
            if (request.NewNailArtistId.HasValue)
            {
                var artist = await _unitOfWork.NailArtistRepository.GetNailArtistWithProfileAsync(request.NewNailArtistId.Value);

                if(artist == null || artist.Status != "Active")
                {
                    return new ApiErrorResult<BookingResponseDTO>("Thợ được chọn không tồn tại hoặc không hoạt động.");
                }
                if(artist.Account.SalonId != request.TargetSalonId)
                {
                    return new ApiErrorResult<BookingResponseDTO>("Thợ được chọn không thuộc chi nhánh đích.");
                }

                var procedures = await _unitOfWork.BookingProcedureRepository.GetProceduresByBookingIdAsync(bookingId);
                var timeline = _bookingSchedulingService.BuildProcedureTimeline(procedures, booking.StartTime);

                var hasConflict = await _bookingSchedulingService.HasCapacityConflictAsync(request.NewNailArtistId.Value, booking.BookingDate, timeline, artist.ConcurrentCapacity);
                if (hasConflict)
                {
                    return new ApiErrorResult<BookingResponseDTO>("Thợ được chọn đã bận trong khung giờ này tại chi nhánh đích.");
                }
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                booking.TransferToSalon(request.TargetSalonId, request.NewNailArtistId, actorId, request.Reason);
                _unitOfWork.BookingRepository.Update(booking);

                var oldProcedures = await _unitOfWork.BookingProcedureRepository.GetProceduresByBookingIdAsync(bookingId, trackChanges: true);
                foreach(var proc in oldProcedures)
                {
                    _unitOfWork.BookingProcedureRepository.Delete(proc);
                }

                await _unitOfWork.SaveChangesAsync();

                var bookingItems = await _unitOfWork.BookingItemRepository.GetBookingItemsByBookingIdAsync(bookingId);
                foreach(var item in bookingItems)
                {
                    await _bookingProcedureService.DuplicateProceduresForBookingItemAsync(item);
                }
                await _unitOfWork.SaveChangesAsync();

                if (booking.NailArtistId.HasValue)
                {
                    var newProcedures = await _unitOfWork.BookingProcedureRepository.GetProceduresByBookingIdAsync(bookingId, trackChanges: true);
                    if (newProcedures.Any())
                    {
                        var newTimeline = _bookingSchedulingService.BuildProcedureTimeline(newProcedures, booking.StartTime);
                        foreach (var segment in newTimeline)
                        {
                            var proc = newProcedures.First(x => x.BookingProcedureId == segment.BookingProcedureId);
                            proc.EstimatedStartTime = segment.StartTime;
                            proc.EstimatedEndTime = segment.EndTime;
                            if (proc.ActiveDuration > 0 && proc.IsMainStep)
                            { 
                                proc.AssignedArtistId = booking.NailArtistId.Value;
                            }
                            _unitOfWork.BookingProcedureRepository.Update(proc);
                        }
                        await _unitOfWork.SaveChangesAsync();
                    }
                }
                await _unitOfWork.CommitTransactionAsync();

                _ = _notificationService.SendNotificationToUserAsync(
                    booking.CustomerId.ToString(),
                    "BookingTransferred",
                    new
                    {
                        BookingId = bookingId,
                        NewSalonName = targetSalon.Name,
                        Message = $"Lịch hẹn của bạn đã được chuyển sang chi nhánh '{targetSalon.Name}'. " +
                      "Vui lòng đến đúng địa điểm mới."
                    });
                var savedBooking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(bookingId);
                var response = _mapper.Map<BookingResponseDTO>(savedBooking);
                return new ApiSuccessResult<BookingResponseDTO>(response, "Chuyển chi nhánh thành công.");
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new ApiErrorResult<BookingResponseDTO>(
                    "Có lỗi hệ thống khi chuyển chi nhánh. Vui lòng thử lại.");
            }
        }
    }
}
