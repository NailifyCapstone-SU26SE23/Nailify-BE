using AutoMapper;
using Microsoft.Extensions.Logging;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs;
using Nailify.Capstone.Application.DTOs.RequestDTOs.WalkInQueueRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;


namespace Nailify.Capstone.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IQRService _qrService;
        private readonly IBookingProcedureService _bookingProcedureService;
        private readonly INailVariantService _nailVariantService;
        private readonly ILoyaltyTierService _loyaltyTierService;
        private readonly ISlotHoldService _slotHoldService;
        private readonly IPromotionService _promotionService;
        private readonly ILogger<BookingService> _logger;
        private readonly IBookingSchedulingService _bookingSchedulingService;
        private readonly IWalkInQueueService _queueService;
        public BookingService(IUnitOfWork unitOfWork, IMapper mapper, IQRService qrService, IBookingProcedureService bookingProcedureService, INailVariantService nailVariantService, ISlotHoldService slotHoldService, ILoyaltyTierService loyaltyTierService, IPromotionService promotionService, ILogger<BookingService> logger, IBookingSchedulingService bookingSchedulingService, IWalkInQueueService queueService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _qrService = qrService;
            _bookingProcedureService = bookingProcedureService;
            _nailVariantService = nailVariantService;
            _loyaltyTierService = loyaltyTierService;
            _slotHoldService = slotHoldService;
            _promotionService = promotionService;
            _logger = logger;
            _bookingSchedulingService = bookingSchedulingService;
            _queueService = queueService;
        }

        public async Task<ApiResult<BookingResponseDTO>> VerifyQrCodeAsync(string qrToken, Guid actorId)
        {
            if (string.IsNullOrEmpty(qrToken))
            {
                return new ApiErrorResult<BookingResponseDTO>("Mã QR không hợp lệ.");
            }

            var parts = qrToken.Split('|');
            if (parts.Length != 3 || parts[0] != "NAILIFY")
            {
                return new ApiErrorResult<BookingResponseDTO>("Định dạng mã QR không đúng.");
            }

            if (!Guid.TryParse(parts[1], out Guid bookingId))
            {
                return new ApiErrorResult<BookingResponseDTO>("Mã đặt lịch trong QR không hợp lệ.");
            }

            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(bookingId, trackChanges: true);
            if (booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Không tìm thấy thông tin đặt lịch.");
            }

            if (booking.Status != BookingStatus.Approved && booking.Status != BookingStatus.CheckedIn)
            {
                return new ApiErrorResult<BookingResponseDTO>($"Đơn đặt lịch không ở trạng thái sẵn sàng để check-in. Trạng thái hiện tại: '{booking.Status}'.");
            }

            var tokenDateStr = parts[2];
            var localBookingDate = (booking.BookingDate.Kind == DateTimeKind.Utc ? booking.BookingDate.AddHours(7) : booking.BookingDate).Date;
            if (localBookingDate.ToString("yyyyMMdd") != tokenDateStr)
            {
                return new ApiErrorResult<BookingResponseDTO>("Ngày đặt lịch không khớp với thông tin trên mã QR.");
            }

            if (booking.Status == BookingStatus.Approved)
            {
                /*
                  booking.Status = BookingStatus.CheckedIn;
                  booking.UpdatedAt = DateTime.UtcNow;

                  var history = new BookingHistory
                  {
                      BookingHistoryId = Guid.NewGuid(),
                      BookingId = booking.BookingId,
                      EventType = "CheckedIn",
                      Payload = "Xác thực mã QR thành công. Trạng thái đơn hàng chuyển sang CheckedIn.",
                      CreatedAt = DateTime.UtcNow
                  };
                */

                booking.CheckInFromQr(actorId);

                _unitOfWork.BookingRepository.Update(booking);
                //await _unitOfWork.BookingHistoryRepository.CreateAsync(history);
                await _unitOfWork.SaveChangesAsync();
            }

            var response = _mapper.Map<BookingResponseDTO>(booking);
            return new ApiSuccessResult<BookingResponseDTO>(response, "Xác thực mã QR thành công. Trạng thái đơn chuyển sang CheckedIn.");
        }

        public async Task<ApiResult<BookingResponseDTO>> CheckInBookingAsync(CheckInRequestDTO request, Guid actorId)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(request.BookingId, trackChanges: true);
            if (booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Không tìm thấy thông tin đặt lịch.");
            }
            if (booking.Status != BookingStatus.Approved && booking.Status != BookingStatus.CheckedIn)
            {
                return new ApiErrorResult<BookingResponseDTO>($"Chỉ có thể check-in đơn đã được xác nhận duyệt ('Approved') hoặc đang check-in. Trạng thái hiện tại: '{booking.Status}'.");
            }
            /*
            booking.CheckInImageUrl = request.CheckInImageUrl;
            booking.Status = BookingStatus.CheckedIn;
            booking.UpdatedAt = DateTime.UtcNow;
            var history = new BookingHistory
            {
                BookingHistoryId = Guid.NewGuid(),
                BookingId = booking.BookingId,
                EventType = "CheckedIn",
                Payload = $"Check-in thành công. Đã chụp trạng thái tay trước khi làm: {request.CheckInImageUrl}",
                CreatedAt = DateTime.UtcNow
            };
            */
            booking.CheckIn(request.CheckInImageUrl, actorId);
            _unitOfWork.BookingRepository.Update(booking);
            //await _unitOfWork.BookingHistoryRepository.CreateAsync(history);
            await _unitOfWork.SaveChangesAsync();
            if (booking.IsLateArrival)
            {
                var originalArtistId = booking.NailArtistId;
                booking.NailArtistId = null;

                var procedures = await _unitOfWork.BookingProcedureRepository.GetProceduresByBookingIdAsync(booking.BookingId);
                foreach (var proc in procedures)
                {
                    proc.AssignedArtistId = null;
                    _unitOfWork.BookingProcedureRepository.Update(proc);
                }

                _unitOfWork.BookingRepository.Update(booking);
                await _unitOfWork.SaveChangesAsync();
                var addToQueueRequest = new AddToQueueRequestDTO
                {
                    SalonId = booking.SalonId,
                    CustomerId = booking.CustomerId,
                    OriginalBookingId = booking.BookingId,
                    GuestName = $"{booking.Customer.User.FirstName} {booking.Customer.User.LastName}",
                    GuestPhone = booking.Customer.User.Phone,
                    RequestNote = "Khách hàng đến muộn -> Tự động chuyển xuống hàng chờ.",
                    AssignedNailArtistId = originalArtistId
                };

                await _queueService.AddToQueueAsync(actorId, addToQueueRequest);
            }
            var response = _mapper.Map<BookingResponseDTO>(booking);
            return new ApiSuccessResult<BookingResponseDTO>(response, booking.IsLateArrival
                            ? "Khách hàng đến muộn -> Tự động chuyển xuống hàng chờ."
                            : "Khách hàng Check-in thành công.");
        }

        public async Task<ApiResult<BookingResponseDTO>> CheckOutBookingAsync(CheckOutRequestDTO request, Guid actorId)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(request.BookingId, trackChanges: true);
            if (booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Không tìm thấy thông tin đặt lịch.");
            }
            if (booking.Status != BookingStatus.ServiceCompleted)
            {
                return new ApiErrorResult<BookingResponseDTO>($"Chỉ có thể check-out thanh toán khi dịch vụ đã làm xong ('ServiceCompleted'). Trạng thái hiện tại; '{booking.Status}'.");
            }

            booking.CheckOut(actorId);
            _unitOfWork.BookingRepository.Update(booking);
            //await _unitOfWork.BookingHistoryRepository.CreateAsync(history);
            await _unitOfWork.SaveChangesAsync();

            var response = _mapper.Map<BookingResponseDTO>(booking);
            return new ApiSuccessResult<BookingResponseDTO>(response, "Khách hàng Check-out thành công.");
        }

        public async Task<ApiResult<BookingResponseDTO>> CreateBookingAsync(Guid customerId, CreateBookingRequestDTO request)
        {
            if (request.BookingItems == null || !request.BookingItems.Any())
            {
                return new ApiErrorResult<BookingResponseDTO>("Vui lòng chọn ít nhất một mẫu móng hoặc dịch vụ.");
            }

            // Tự động kiểm tra và cưỡng chế thợ khi đặt lịch mẫu custom
            var customNailItem = request.BookingItems.FirstOrDefault(x => x.CustomerNailRequestId.HasValue);
            if (customNailItem != null)
            {
                var customNailRequest = await _unitOfWork.CustomerNailRequestRepository.GetByIdAsync(customNailItem.CustomerNailRequestId!.Value);
                if (customNailRequest == null)
                {
                    return new ApiErrorResult<BookingResponseDTO>($"Không tìm thấy yêu cầu mẫu móng custom ID {customNailItem.CustomerNailRequestId.Value}");
                }

                if (customNailRequest.SalonId != request.SalonId ||
                    (customNailRequest.Status != CustomerNailStatus.Approved && customNailRequest.Status != CustomerNailStatus.Quoted))
                {
                    return new ApiErrorResult<BookingResponseDTO>("Yêu cầu mẫu móng custom chưa được duyệt báo giá hoặc không thuộc chi nhánh này.");
                }

                var customNail = await _unitOfWork.CustomerNailRepository.GetCustomerNailDetailAsync(customNailRequest.CustomerNailId);
                if (customNail == null)
                {
                    return new ApiErrorResult<BookingResponseDTO>("Không tìm thấy mẫu móng custom của yêu cầu này.");
                }

                if (!customNailRequest.ApprovedArtistId.HasValue)
                {
                    return new ApiErrorResult<BookingResponseDTO>($"Mẫu móng custom '{customNail.Name}' không có thông tin thợ duyệt.");
                }

                var approvedArtistId = customNailRequest.ApprovedArtistId.Value;

                if (request.NailArtistId.HasValue && request.NailArtistId.Value != approvedArtistId)
                {
                    var artist = await _unitOfWork.NailArtistRepository.GetNailArtistWithProfileAsync(approvedArtistId);
                    string artistName = artist != null ? $"{artist.Account.FirstName} {artist.Account.LastName}" : "thợ ban đầu";
                    return new ApiErrorResult<BookingResponseDTO>($"Mẫu móng custom này bắt buộc phải chọn thợ {artistName} (người đã duyệt mẫu móng).");
                }

                if (!request.NailArtistId.HasValue)
                {
                    request.NailArtistId = approvedArtistId;
                }
            }

            var bookingId = Guid.NewGuid();
            var calculation = await BuildBookingItemsAsync(request.BookingItems, bookingId, request.SalonId, request.NailArtistId);
            if (!calculation.IsSucceeded)
            {
                return new ApiErrorResult<BookingResponseDTO>(calculation.ErrorMessage!);
            }

            var loyaltyResult = await _loyaltyTierService.GetMyLoyaltyAsync(customerId);
            if (!loyaltyResult.IsSucceeded)
            {
                return new ApiErrorResult<BookingResponseDTO>(loyaltyResult.Message);
            }

            var bookingItems = calculation.Items;
            var applicablePromotions = await _promotionService.GetApplicablePromotionsAsync(
                customerId,
                bookingItems,
                request.SelectedPromotionIds);
            var (promotionDiscountAmount, appliedPromotionDiscounts) =
                await _promotionService.CalculateDiscountsAsync(new Booking
                {
                    BookingId = bookingId,
                    CustomerId = customerId,
                    BookingItems = bookingItems
                }, applicablePromotions);
            var loyaltyDiscountAmount = decimal.Round(
                calculation.Price * loyaltyResult.Data.LoyaltyTier.DiscountRate,
                0,
                MidpointRounding.AwayFromZero);

            if (loyaltyDiscountAmount > 0)
            {
                appliedPromotionDiscounts.Add(new BookingDiscount
                {
                    BookingId = bookingId,
                    Name = $"{loyaltyResult.Data.LoyaltyTier.Name} Tier",
                    DiscountAmount = loyaltyDiscountAmount,
                    IsAutoApplied = true,
                    AppliedDate = DateTime.UtcNow,
                    LoyaltyTierId = loyaltyResult.Data.LoyaltyTier.LoyaltyTierId
                });
            }
            var totalDuration = calculation.Duration;
            var totalDiscountAmount = loyaltyDiscountAmount + promotionDiscountAmount;
            var bookingPrice = new BookingPriceResponseDTO
            {
                Price = calculation.Price,
                Discount = -totalDiscountAmount,
                TotalPrice = Math.Max(0, calculation.Price - totalDiscountAmount)
            };
            string qrCodeToken = $"NAILIFY|{bookingId}|{request.BookingDate:yyyyMMdd}";
            string qrCodeBase64 = _qrService.GenerateQRCode(qrCodeToken);

            var booking = _mapper.Map<Booking>(request);
            booking.BookingId = bookingId;
            booking.CustomerId = customerId;
            booking.Price = bookingPrice.Price;
            booking.Discount = bookingPrice.Discount;
            booking.TotalPrice = bookingPrice.TotalPrice;
            booking.TotalDuration = totalDuration;
            booking.QRCode = qrCodeBase64;
            booking.Status = BookingStatus.Pending;
            booking.BookingItems = bookingItems;
            booking.BookingDiscounts = appliedPromotionDiscounts;

            // BẮT ĐẦU TRANSACTION AN TOÀN TRÁNH RACE CONDITION KHI TẠO BOOKING
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                if (request.NailArtistId.HasValue)
                {
                    // 1. Khóa dòng Thợ nail bằng FOR UPDATE
                    var artist = await _unitOfWork.NailArtistRepository.GetArtistWithLockAsync(request.NailArtistId.Value);
                    if (artist == null || artist.Status != "Active")
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        return new ApiErrorResult<BookingResponseDTO>("Thợ nail không hoạt động hoặc không tồn tại.");
                    }
                    // 2. Validate Hold Token (nếu có)
                    if (!string.IsNullOrEmpty(request.HoldToken))
                    {
                        var isValid = await _slotHoldService.ValidateHoldTokenAsync(request.HoldToken, customerId, request.NailArtistId.Value, request.BookingDate, request.StartTime);
                        if (!isValid)
                        {
                            await _unitOfWork.RollbackTransactionAsync();
                            return new ApiErrorResult<BookingResponseDTO>("Mã giữ chỗ không hợp lệ hoặc đã hết hạn.");
                        }
                    }
                    else
                    {
                        // Nếu đặt trực tiếp không giữ chỗ, bắt buộc kiểm tra xem slot này có đang bị người khác HOLD không
                        var targetEnd = request.StartTime.Add(TimeSpan.FromMinutes(totalDuration));
                        var isHeld = await _slotHoldService.IsSlotHeldAsync(request.NailArtistId.Value, request.BookingDate, request.StartTime, targetEnd);
                        if (isHeld)
                        {
                            await _unitOfWork.RollbackTransactionAsync();
                            return new ApiErrorResult<BookingResponseDTO>("Khoảng thời gian này đang có người giữ chỗ. Vui lòng chọn giờ khác.");
                        }
                    }
                    // 3. Kiểm tra xem có bị xung đột Capacity thực tế không
                    var mockProcedures = await _bookingSchedulingService.GenerateMockBookingProceduresAsync(
                        request.BookingItems.ToList(),
                        request.SalonId);
                    var timeline = _bookingSchedulingService.BuildProcedureTimeline(
                        mockProcedures,
                        request.StartTime);
                    var isConflict = await _bookingSchedulingService.HasCapacityConflictAsync(
                        request.NailArtistId.Value,
                        request.BookingDate,
                        timeline,
                        artist.ConcurrentCapacity);
                    if (isConflict)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        return new ApiErrorResult<BookingResponseDTO>("Khoảng thời gian này thợ đã bận, xin chọn giờ khác.");
                    }
                }
                /*
                var history = new BookingHistory
                {
                    BookingHistoryId = Guid.NewGuid(),
                    BookingId = booking.BookingId,
                    EventType = "BookingCreated",
                    Payload = $"Đơn đặt lịch được tạo thành công bởi khách hàng. Mã QR (Base64) đã được khởi tạo.",
                    ActorId = customerId,
                    CreatedAt = DateTime.UtcNow
                };
                */
                booking.Created(customerId);


                await _unitOfWork.BookingRepository.CreateAsync(booking);
                await _promotionService.UpdateUsageAsync(customerId, appliedPromotionDiscounts);
                //await _unitOfWork.BookingHistoryRepository.CreateAsync(history);
                await _unitOfWork.SaveChangesAsync();
                // 5. Giải phóng Hold Token
                if (!string.IsNullOrEmpty(request.HoldToken))
                {
                    await _slotHoldService.ConsumeHoldAsync(request.HoldToken);
                }
                // Tạo các quy trình (Procedures) mặc định cho booking
                foreach (var item in booking.BookingItems)
                {
                    /* if (item.NailVariantId.HasValue)
                    { */
                    await _bookingProcedureService.DuplicateProceduresForBookingItemAsync(item);
                    //}
                }
                await _unitOfWork.SaveChangesAsync();

                // Tính toán timeline và gán ngay lập tức nếu đã chọn thợ
                if (booking.NailArtistId.HasValue)
                {
                    var procedures = await _unitOfWork.BookingProcedureRepository.GetProceduresByBookingIdAsync(booking.BookingId, trackChanges: true);
                    if (procedures.Any())
                    {
                        var timeline = _bookingSchedulingService.BuildProcedureTimeline(procedures, booking.StartTime);
                        foreach (var segment in timeline)
                        {
                            var procedure = procedures.First(x => x.BookingProcedureId == segment.BookingProcedureId);
                            procedure.EstimatedStartTime = segment.StartTime;
                            procedure.EstimatedEndTime = segment.EndTime;
                            if (procedure.ActiveDuration > 0)
                            {
                                procedure.AssignedArtistId = booking.NailArtistId.Value;
                            }
                            _unitOfWork.BookingProcedureRepository.Update(procedure);
                        }
                        await _unitOfWork.SaveChangesAsync();
                    }
                }
                // Commit Transaction thành công giải phóng khóa dòng
                await _unitOfWork.CommitTransactionAsync();
                var savedBooking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(booking.BookingId);
                var response = _mapper.Map<BookingResponseDTO>(savedBooking);
                return new ApiSuccessResult<BookingResponseDTO>(response, "Tạo đơn đặt lịch thành công.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Lỗi xảy ra khi CreateBookingAsync");
                return new ApiErrorResult<BookingResponseDTO>("Có lỗi hệ thống xảy ra khi lưu đơn hàng.");
            }
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
                                CanOverlap = x.Procedure.CanOverlap
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
                        var salonId = artist.Account.SalonId ?? Guid.Empty;

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

            var timeSlots = new List<TimeSlotResponseDTO>();
            var candidateStart = schedule.ShiftStart;
            var interval = TimeSpan.FromMinutes(15);
            int totalDuration = mockProcedures.Sum(x => x.Duration);
            while (candidateStart.Add(TimeSpan.FromMinutes(totalDuration)) <= schedule.ShiftEnd)
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
                bool isAvailable = !isConflict;
                bool isHeld = false;
                // C. Nếu không conflict lịch trong DB, kiểm tra tiếp trạng thái giữ chỗ tạm thời (Redis Hold)
                if (isAvailable)
                {
                    isHeld = await _slotHoldService.IsSlotHeldAsync(
                        request.NailArtistId,
                        request.BookingDate,
                        candidateStart,
                        candidateStart.Add(TimeSpan.FromMinutes(totalDuration))
                    );
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

            foreach (var artist in qualifiedArtists)
            {
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

        public async Task<ApiResult<BookingResponseDTO>> UpdateBookingAsync(Guid bookingId, UpdateBookingRequestDTO request, Guid actorId)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(bookingId);
            if (booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Không tìm thấy thông tin đặt lịch.");
            }

            //if (booking.Status != BookingStatus.Pending)
            //{
            //    return new ApiErrorResult<BookingResponseDTO>("Không thể cập nhật đơn đặt lịch đã được xử lý hoặc đã hủy.");
            //}

            if (request.BookingItems == null || !request.BookingItems.Any())
            {
                return new ApiErrorResult<BookingResponseDTO>("Vui lòng chọn ít nhất một mẫu móng hoặc dịch vụ.");
            }

            decimal oldPrice = booking.TotalPrice ?? 0;
            int oldDuration = booking.TotalDuration;

            int totalDuration = 0;
            decimal totalPrice = 0;
            var bookingItems = new List<BookingItem>();

            foreach (var x in request.BookingItems)
            {
                var item = new BookingItem
                {
                    BookingItemId = Guid.NewGuid(),
                    BookingId = bookingId,
                    Quantity = x.Quantity,
                    ServiceId = x.ServiceId,
                    NailVariantId = x.NailVariantId,
                    CustomerNailRequestId = x.CustomerNailRequestId
                };

                if (!x.NailVariantId.HasValue && !x.ServiceId.HasValue && !x.CustomerNailRequestId.HasValue)
                {
                    return new ApiErrorResult<BookingResponseDTO>("Mỗi mục đặt lịch phải chứa ít nhất một dịch vụ, một mẫu nail hoặc một mẫu custom.");
                }
                decimal itemPrice = 0;
                int itemDuration = 0;

                if (x.NailVariantId.HasValue)
                {
                    var variant = await _unitOfWork.NailVariantRepository.GetByIdAsync(x.NailVariantId.Value);
                    if (variant != null)
                    {
                        itemPrice += variant.Price;
                        itemDuration += (variant.Duration ?? 60);
                    }
                    else
                    {
                        return new ApiErrorResult<BookingResponseDTO>($"Không tìm thấy mẫu nail có ID {x.NailVariantId.Value}");
                    }
                }

                if (x.ServiceId.HasValue)
                {
                    var service = await _unitOfWork.ServicesRepository.GetByIdAsync(x.ServiceId.Value);
                    if (service != null)
                    {
                        itemPrice += service.Price;
                        itemDuration += service.Duration;
                    }
                    else
                    {
                        return new ApiErrorResult<BookingResponseDTO>($"Không tìm thấy dịch vụ có ID {x.ServiceId.Value}");
                    }
                }

                if (x.CustomerNailRequestId.HasValue)
                {
                    var customNailRequest = await _unitOfWork.CustomerNailRequestRepository.GetByIdAsync(x.CustomerNailRequestId.Value);
                    if (customNailRequest == null)
                    {
                        return new ApiErrorResult<BookingResponseDTO>($"Không tìm thấy yêu cầu mẫu móng custom ID {x.CustomerNailRequestId.Value}");
                    }

                    if (customNailRequest.SalonId != booking.SalonId ||
                        (customNailRequest.Status != CustomerNailStatus.Approved && customNailRequest.Status != CustomerNailStatus.Quoted))
                    {
                        return new ApiErrorResult<BookingResponseDTO>("Yêu cầu mẫu móng custom chưa được duyệt báo giá hoặc không thuộc chi nhánh này.");
                    }

                    var customNail = await _unitOfWork.CustomerNailRepository.GetCustomerNailDetailAsync(customNailRequest.CustomerNailId);
                    if (customNail == null)
                    {
                        return new ApiErrorResult<BookingResponseDTO>("Không tìm thấy mẫu móng custom của yêu cầu này.");
                    }

                    itemPrice += (customNail.Price ?? 0) + (customNailRequest.Price ?? 0);
                    itemDuration += (customNail.Duration ?? 60) + (customNailRequest.Duration ?? 0);
                }

                item.Price = itemPrice;
                item.DiscountAmount = 0;
                item.FinalPrice = itemPrice * Math.Max(item.Quantity, 1);
                item.Duration = itemDuration;

                totalDuration += item.Duration * Math.Max(item.Quantity, 1);
                totalPrice += item.FinalPrice;

                bookingItems.Add(item);
            }

            if (request.NailArtistId.HasValue)
            {
                var artist = await _unitOfWork.NailArtistRepository.GetByIdAsync(request.NailArtistId.Value);
                int capacity = artist?.ConcurrentCapacity ?? 1;
                var mockProcs = await _bookingSchedulingService.GenerateMockBookingProceduresAsync(request.BookingItems.ToList(), booking.SalonId);
                var timeline = _bookingSchedulingService.BuildProcedureTimeline(mockProcs, request.StartTime);
                var isConflict = await _bookingSchedulingService.HasCapacityConflictAsync(
                request.NailArtistId.Value, request.BookingDate, timeline, capacity);
                if (isConflict)
                {
                    return new ApiErrorResult<BookingResponseDTO>("Khoảng thời gian này thợ đã bận, xin chọn giờ khác.");
                }
            }

            // Xóa các items cũ khỏi DB trước
            var oldItems = await _unitOfWork.BookingItemRepository.GetBookingItemsByBookingIdAsync(bookingId);
            foreach (var oldItem in oldItems)
            {
                oldItem.Booking = null!;
                oldItem.NailVariant = null;
                oldItem.Service = null;
                oldItem.CustomerNailRequest = null;
                _unitOfWork.BookingItemRepository.Delete(oldItem);
            }

            booking.BookingDate = request.BookingDate;
            booking.StartTime = request.StartTime;
            booking.NailArtistId = request.NailArtistId;
            var priceResult = await CalculateDiscountedPriceAsync(booking.CustomerId, totalPrice);
            if (!priceResult.IsSucceeded)
            {
                return new ApiErrorResult<BookingResponseDTO>(priceResult.ErrorMessage!);
            }

            booking.Price = totalPrice;
            booking.Discount = -priceResult.DiscountAmount;
            booking.TotalPrice = priceResult.TotalPrice;
            booking.TotalDuration = totalDuration;
            booking.UpdatedAt = DateTime.UtcNow;

            // Clear list trong memory của booking
            booking.BookingItems.Clear();

            booking.Updated(oldPrice, oldDuration, actorId);

            booking.Customer = null!;
            booking.Salon = null!;
            booking.NailArtist = null;
            booking.BookingHistories.Clear();

            _unitOfWork.BookingRepository.Update(booking);

            foreach (var item in bookingItems)
            {
                await _unitOfWork.BookingItemRepository.CreateAsync(item);
            }

            await _unitOfWork.SaveChangesAsync();

            var savedBooking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(booking.BookingId);
            var response = _mapper.Map<BookingResponseDTO>(savedBooking);
            return new ApiSuccessResult<BookingResponseDTO>(response, "Cập nhật đơn đặt lịch thành công.");
        }

        public async Task<ApiResult<BookingResponseDTO>> CancelBookingAsync(Guid bookingId, Guid customerId, CancelBookingRequestDTO request)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(bookingId, trackChanges: true);
            if (booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Đơn đặt lịch không tồn tại.");
            }

            // if (booking.CustomerId != customerId)
            // {
            //     return new ApiErrorResult<BookingResponseDTO>("Bạn không có quyền hủy lịch hẹn của người khác.");
            // }

            if (booking.Status != BookingStatus.Pending && booking.Status != BookingStatus.Approved)
            {
                return new ApiErrorResult<BookingResponseDTO>($"Chỉ được hủy đơn ở trạng thái 'Pending' hoặc 'Approved'. Trạng thái hiện tại: '{booking.Status}'.");
            }

            booking.Cancel(customerId, request.Reason);
            _unitOfWork.BookingRepository.Update(booking);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<BookingResponseDTO>(booking);
            return new ApiSuccessResult<BookingResponseDTO>(response, "Hủy đơn đặt lịch thành công.");
        }

        public async Task<ApiResult<BookingResponseDTO>> ConfirmBookingAsync(Guid bookingId, Guid actorId)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(bookingId, trackChanges: true);
            if (booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Đơn đặt lịch không tồn tại.");
            }
            if (booking.Status != BookingStatus.Pending)
            {
                return new ApiErrorResult<BookingResponseDTO>($"Chỉ có thể xác nhận đơn ở trạng thái 'Pending'. Trạng thái hiện tại: '{booking.Status}'.");
            }
            booking.Confirm(actorId);
            _unitOfWork.BookingRepository.Update(booking);
            var procedures = await _unitOfWork.BookingProcedureRepository.GetProceduresByBookingIdAsync(bookingId, trackChanges: true);
            if (procedures.Any() && booking.NailArtistId.HasValue)
            {
                // 1. Tính toán Timeline thực tế bắt đầu từ StartTime
                var timeline = _bookingSchedulingService.BuildProcedureTimeline(procedures, booking.StartTime);
                // 2. Cập nhật Estimated time và gán AssignedArtist cho các công đoạn có ActiveDuration > 0
                foreach (var segment in timeline)
                {
                    var procedure = procedures.First(x => x.BookingProcedureId == segment.BookingProcedureId);

                    procedure.EstimatedStartTime = segment.StartTime;
                    procedure.EstimatedEndTime = segment.EndTime;
                    // Nếu công đoạn này thợ cần thao tác (ActiveDuration > 0), gán AssignedArtistId
                    if (procedure.ActiveDuration > 0)
                    {
                        procedure.AssignedArtistId = booking.NailArtistId.Value;
                    }

                    _unitOfWork.BookingProcedureRepository.Update(procedure);
                }
            }
            await _unitOfWork.SaveChangesAsync();
            /*
            var savedBooking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(booking.BookingId);
            */
            var response = _mapper.Map<BookingResponseDTO>(booking);
            return new ApiSuccessResult<BookingResponseDTO>(response, "Duyệt đơn đặt lịch thành công.");
        }

        public async Task<ApiResult<BookingResponseDTO>> ManualCheckInBookingAsync(Guid bookingId, Guid actorId)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(bookingId, trackChanges: true);
            if (booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Đơn đặt lịch không tồn tại.");
            }
            /*
            booking.Status = BookingStatus.CheckedIn;
            booking.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.BookingRepository.Update(booking);
            var history = new BookingHistory
            {
                BookingHistoryId = Guid.NewGuid(),
                BookingId = booking.BookingId,
                EventType = "BookingCheckedIn",
                Payload = "Tiếp tân checkin lịch hẹn bằng tay.",
                ActorId = actorId == Guid.Empty ? null : actorId,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.BookingHistoryRepository.CreateAsync(history);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<BookingResponseDTO>(booking);
            return new ApiSuccessResult<BookingResponseDTO>(response, "Checkin lịch hẹn thành công.");
            */
            booking.CheckInWithoutImage(actorId);
            _unitOfWork.BookingRepository.Update(booking);
            await _unitOfWork.SaveChangesAsync();
            if (booking.IsLateArrival)
            {
                var originalArtistId = booking.NailArtistId;
                booking.NailArtistId = null;

                var procedures = await _unitOfWork.BookingProcedureRepository.GetProceduresByBookingIdAsync(booking.BookingId, trackChanges: true);
                foreach (var proc in procedures)
                {
                    proc.AssignedArtistId = null;
                    _unitOfWork.BookingProcedureRepository.Update(proc);
                }

                _unitOfWork.BookingRepository.Update(booking);
                await _unitOfWork.SaveChangesAsync();
                var addToQueueRequest = new AddToQueueRequestDTO
                {
                    SalonId = booking.SalonId,
                    CustomerId = booking.CustomerId,
                    OriginalBookingId = booking.BookingId,
                    GuestName = $"{booking.Customer.User.FirstName} {booking.Customer.User.LastName}",
                    GuestPhone = booking.Customer.User.Phone,
                    RequestNote = "Khách hàng đến muộn -> Tự động chuyển xuống hàng chờ.",
                    AssignedNailArtistId = originalArtistId 
                };

                await _queueService.AddToQueueAsync(actorId, addToQueueRequest);
            }
            var response = _mapper.Map<BookingResponseDTO>(booking);
            return new ApiSuccessResult<BookingResponseDTO>(response, booking.IsLateArrival
                ? "Khách hàng đến muộn -> Tự động chuyển xuống hàng chờ."
                : "Checkin lịch hẹn thành công.");
        }

        public async Task<ApiResult<BookingResponseDTO>> RejectBookingAsync(Guid bookingId, Guid actorId, RejectRequestDTO request)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(bookingId, trackChanges: true);
            if (booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Đơn đặt lịch không tồn tại.");
            }
            if (booking.Status != BookingStatus.Pending)
            {
                return new ApiErrorResult<BookingResponseDTO>($"Chỉ có thể từ chối đơn ở trạng thái 'Pending'. Trạng thái hiện tại: '{booking.Status}'.");
            }
            booking.Reject(actorId, request.Reason);
            _unitOfWork.BookingRepository.Update(booking);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<BookingResponseDTO>(booking);
            return new ApiSuccessResult<BookingResponseDTO>(response, "Từ chối đơn đặt lịch thành công.");
        }

        public async Task<ApiResult<BookingResponseDTO>> StartServiceAsync(Guid bookingId, Guid actorId)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(bookingId, trackChanges: true);
            if (booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Đơn đặt lịch không tồn tại.");
            }
            if (booking.Status != BookingStatus.CheckedIn)
            {
                return new ApiErrorResult<BookingResponseDTO>($"Chỉ có thể bắt đầu làm khi khách đã 'CheckedIn'. Trạng thái hiện tại: '{booking.Status}'.");
            }
            booking.StartService(actorId);
            _unitOfWork.BookingRepository.Update(booking);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<BookingResponseDTO>(booking);
            return new ApiSuccessResult<BookingResponseDTO>(response, "Bắt đầu làm móng thành công.");
        }

        public async Task<ApiResult<PagedList<BookingResponseDTO>>> GetMyBookingsAsync(Guid customerId, int pageNumber, int pageSize, DateTime? startDate = null, DateTime? endDate = null, BookingStatus? status = null)
        {
            var bookings = await _unitOfWork.BookingRepository.GetBookingsByCustomerAsync(customerId, pageNumber, pageSize, startDate, endDate, status);
            var response = MapPagedBookings(bookings, pageNumber, pageSize);
            return new ApiSuccessResult<PagedList<BookingResponseDTO>>(response, "Lấy danh sách đặt lịch của khách hàng thành công.");
        }

        public async Task<ApiResult<PagedList<BookingResponseDTO>>> GetBookingsBySalonAsync(Guid salonId, int pageNumber, int pageSize, DateTime? startDate = null, DateTime? endDate = null, BookingStatus? status = null, string? search = null)
        {
            var bookings = await _unitOfWork.BookingRepository.GetBookingsBySalonAsync(salonId, pageNumber, pageSize, startDate, endDate, status, search);
            var response = MapPagedBookings(bookings, pageNumber, pageSize);
            return new ApiSuccessResult<PagedList<BookingResponseDTO>>(response, "Lấy danh sách đặt lịch của Salon thành công.");
        }

        public async Task<ApiResult<PagedList<BookingResponseDTO>>> GetBookingsByArtistAsync(Guid artistId, int pageNumber, int pageSize, DateTime? startDate = null, DateTime? endDate = null, BookingStatus? status = null, string? search = null)
        {
            var bookings = await _unitOfWork.BookingRepository.GetBookingsByArtistAsync(artistId, pageNumber, pageSize, startDate, endDate, status, search);
            var response = MapPagedBookings(bookings, pageNumber, pageSize);
            return new ApiSuccessResult<PagedList<BookingResponseDTO>>(response, "Lấy danh sách đặt lịch của Thợ làm móng thành công.");
        }

        public async Task<ApiResult<BookingResponseDTO>> GetBookingByIdAsync(Guid bookingId)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(bookingId);
            if (booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Không tìm thấy thông tin đặt lịch.");
            }
            var response = _mapper.Map<BookingResponseDTO>(booking);
            return new ApiSuccessResult<BookingResponseDTO>(response, "Lấy thông tin chi tiết đặt lịch thành công.");
        }

        public async Task<ApiResult<BookingPriceResponseDTO>> CalculateBookingPriceAsync(
    Guid customerId,
    IEnumerable<BookingItemRequestDTO> bookingItems,
    List<int>? selectedPromotionIds = null)
        {
            var calculation = await BuildBookingItemsAsync(bookingItems, Guid.Empty, null);
            if (!calculation.IsSucceeded)
            {
                return new ApiErrorResult<BookingPriceResponseDTO>(calculation.ErrorMessage!);
            }

            var loyaltyResult = await _loyaltyTierService.GetMyLoyaltyAsync(customerId);
            if (!loyaltyResult.IsSucceeded)
            {
                return new ApiErrorResult<BookingPriceResponseDTO>(loyaltyResult.Message);
            }

            var bookingItemsList = calculation.Items.ToList();
            var applicablePromotions = await _promotionService.GetApplicablePromotionsAsync(
                customerId,
                bookingItemsList,
                selectedPromotionIds);

            var (promotionDiscountAmount, appliedPromotionDiscounts) =
                await _promotionService.CalculateDiscountsAsync(new Booking
                {
                    CustomerId = customerId,
                    BookingItems = bookingItemsList
                }, applicablePromotions);

            var loyaltyDiscountAmount = decimal.Round(
                calculation.Price * loyaltyResult.Data.LoyaltyTier.DiscountRate,
                0,
                MidpointRounding.AwayFromZero);

            var totalDiscountAmount = promotionDiscountAmount + loyaltyDiscountAmount;
            var finalPrice = Math.Max(0, calculation.Price - totalDiscountAmount);

            var response = new BookingPriceResponseDTO
            {
                Price = calculation.Price,
                Discount = -totalDiscountAmount,
                TotalPrice = finalPrice,
                TotalDuration = calculation.Duration,
                DiscountBreakdown = new List<DiscountBreakdownDTO>()
            };

            foreach (var discount in appliedPromotionDiscounts)
            {
                response.DiscountBreakdown.Add(new DiscountBreakdownDTO
                {
                    Name = discount.Name,
                    Amount = discount.DiscountAmount,
                    Type = "Promotion"
                });
            }

            if (loyaltyDiscountAmount > 0)
            {
                response.DiscountBreakdown.Add(new DiscountBreakdownDTO
                {
                    Name = $"{loyaltyResult.Data.LoyaltyTier.Name} Tier",
                    Amount = loyaltyDiscountAmount,
                    Type = "Loyalty"
                });
            }

            return new ApiSuccessResult<BookingPriceResponseDTO>(
                response,
                "Tính giá đặt lịch thành công.");
        }

        private async Task<BookingItemsCalculation> BuildBookingItemsAsync(
            IEnumerable<BookingItemRequestDTO> requests,
            Guid bookingId,
            Guid? salonId,
            Guid? customerPassedArtistId = null)
        {
            var requestItems = requests?.ToList() ?? new List<BookingItemRequestDTO>();
            if (!requestItems.Any())
            {
                return BookingItemsCalculation.Failure("Vui lòng chọn ít nhất một mẫu móng hoặc dịch vụ.");
            }

            var items = _mapper.Map<List<BookingItem>>(requestItems);
            decimal totalPrice = 0;
            var totalDuration = 0;

            foreach (var item in items)
            {
                item.BookingId = bookingId;
                item.Quantity = Math.Max(item.Quantity, 1);

                if (!item.NailVariantId.HasValue && !item.ServiceId.HasValue && !item.CustomerNailRequestId.HasValue)
                {
                    return BookingItemsCalculation.Failure(
                        "Mỗi mục đặt lịch phải chứa ít nhất một dịch vụ, một mẫu nail hoặc một mẫu custom.");
                }

                decimal unitPrice = 0;
                var unitDuration = 0;

                if (item.CustomerNailRequestId.HasValue)
                {
                    var customNailRequest = await _unitOfWork.CustomerNailRequestRepository.GetByIdAsync(item.CustomerNailRequestId.Value);

                    if (customNailRequest == null)
                    {
                        return BookingItemsCalculation.Failure(
                            $"Không tìm thấy yêu cầu mẫu móng custom ID {item.CustomerNailRequestId.Value}");
                    }

                    if (salonId.HasValue && salonId.Value != Guid.Empty && customNailRequest.SalonId != salonId.Value)
                    {
                        return BookingItemsCalculation.Failure(
                            "Yêu cầu mẫu móng custom không thuộc chi nhánh này.");
                    }

                    if (customNailRequest.Status != CustomerNailStatus.Approved && customNailRequest.Status != CustomerNailStatus.Quoted)
                    {
                        return BookingItemsCalculation.Failure(
                            "Yêu cầu mẫu móng custom chưa được duyệt báo giá hoặc đã bị từ chối.");
                    }

                    var customNail = await _unitOfWork.CustomerNailRepository.GetCustomerNailDetailAsync(customNailRequest.CustomerNailId);
                    if (customNail == null)
                    {
                        return BookingItemsCalculation.Failure(
                            "Không tìm thấy mẫu móng custom của yêu cầu này.");
                    }

                    if (customerPassedArtistId.HasValue && customerPassedArtistId.Value != Guid.Empty)
                    {
                        if (customerPassedArtistId.Value != customNailRequest.ApprovedArtistId)
                        {
                            var artist = await _unitOfWork.NailArtistRepository.GetNailArtistWithProfileAsync(customNailRequest.ApprovedArtistId ?? Guid.Empty);
                            string artistName = artist != null ? $"{artist.Account.FirstName} {artist.Account.LastName}" : "thợ ban đầu";
                            return BookingItemsCalculation.Failure(
                                $"Mẫu móng custom '{customNail.Name}' chỉ được phép đặt lịch với thợ {artistName} (người đã thẩm định mẫu).");
                        }
                    }

                    unitPrice += (customNail.Price ?? 0) + (customNailRequest.Price ?? 0);
                    unitDuration += (customNail.Duration ?? 60) + (customNailRequest.Duration ?? 0);
                }

                if (item.NailVariantId.HasValue)
                {
                    var variant = await _nailVariantService.GetNailVariantByIdAsync(item.NailVariantId.Value);
                    if (variant?.Data == null)
                    {
                        return BookingItemsCalculation.Failure(
                            $"Không tìm thấy mẫu nail có ID {item.NailVariantId.Value}");
                    }

                    unitPrice += variant.Data.Price;
                    unitDuration += variant.Data.Duration ?? 60;
                }

                if (item.ShapeMethodConfigId.HasValue)
                {
                    var shapeMethodConfig = await _unitOfWork.ShapeMethodConfigRepository.GetByIdAsync(item.ShapeMethodConfigId.Value);
                    if (shapeMethodConfig == null)
                    {
                        return BookingItemsCalculation.Failure(
                            $"Khong tim thay cau hinh cach lam dang mong ID {item.ShapeMethodConfigId.Value}");
                    }

                    if (item.NailVariantId.HasValue)
                    {
                        var variantEntity = await _unitOfWork.NailVariantRepository.GetByIdAsync(item.NailVariantId.Value);
                        if (variantEntity?.NailShapeId != shapeMethodConfig.NailShapeId)
                        {
                            return BookingItemsCalculation.Failure(
                                "Cau hinh cach lam khong thuoc dang mong cua mau nail da chon.");
                        }
                    }

                    if (item.CustomerNailRequestId.HasValue)
                    {
                        var customNailRequest = await _unitOfWork.CustomerNailRequestRepository.GetByIdAsync(item.CustomerNailRequestId.Value);
                        var customNail = customNailRequest == null
                            ? null
                            : await _unitOfWork.CustomerNailRepository.GetByIdAsync(customNailRequest.CustomerNailId);

                        if (customNail?.NailShapeId != shapeMethodConfig.NailShapeId)
                        {
                            return BookingItemsCalculation.Failure(
                                "Cau hinh cach lam khong thuoc dang mong cua mau custom da chon.");
                        }
                    }

                    unitPrice += shapeMethodConfig.Price;
                    unitDuration += shapeMethodConfig.Duration;
                }

                if (item.ServiceId.HasValue)
                {
                    var service = await _unitOfWork.ServicesRepository.GetByIdAsync(item.ServiceId.Value);
                    if (service == null)
                    {
                        return BookingItemsCalculation.Failure(
                            $"Không tìm thấy dịch vụ có ID {item.ServiceId.Value}");
                    }

                    unitPrice += service.Price;
                    unitDuration += service.Duration;
                }

                item.Price = unitPrice;
                item.DiscountAmount = 0;
                item.FinalPrice = unitPrice * Math.Max(item.Quantity, 1);
                item.Duration = unitDuration;
                totalPrice += unitPrice * item.Quantity;
                totalDuration += unitDuration * item.Quantity;
            }

            return BookingItemsCalculation.Success(items, totalPrice, totalDuration);
        }

        private async Task<DiscountedPriceCalculation> CalculateDiscountedPriceAsync(
            Guid customerId,
            decimal price)
        {
            var loyaltyResult = await _loyaltyTierService.GetMyLoyaltyAsync(customerId);
            if (!loyaltyResult.IsSucceeded)
            {
                return DiscountedPriceCalculation.Failure(loyaltyResult.Message);
            }

            var discountRate = loyaltyResult.Data.LoyaltyTier.DiscountRate;
            var discountAmount = decimal.Round(price * discountRate, 0, MidpointRounding.AwayFromZero);
            return DiscountedPriceCalculation.Success(discountAmount, price - discountAmount);
        }

        private sealed record BookingItemsCalculation(
            bool IsSucceeded,
            List<BookingItem> Items,
            decimal Price,
            int Duration,
            string? ErrorMessage)
        {
            public static BookingItemsCalculation Success(
                List<BookingItem> items,
                decimal price,
                int duration) => new(true, items, price, duration, null);

            public static BookingItemsCalculation Failure(string message)
                => new(false, new List<BookingItem>(), 0, 0, message);
        }

        private sealed record DiscountedPriceCalculation(
            bool IsSucceeded,
            decimal DiscountAmount,
            decimal TotalPrice,
            string? ErrorMessage)
        {
            public static DiscountedPriceCalculation Success(decimal discountAmount, decimal totalPrice)
                => new(true, discountAmount, totalPrice, null);

            public static DiscountedPriceCalculation Failure(string message)
                => new(false, 0, 0, message);
        }

        private PagedList<BookingResponseDTO> MapPagedBookings(PagedList<Booking> pagedBookings, int pageNumber, int pageSize)
        {
            var mappedItems = _mapper.Map<List<BookingResponseDTO>>(pagedBookings.Items);
            return new PagedList<BookingResponseDTO>(mappedItems, pagedBookings.MetaData.TotalItems, pageNumber, pageSize);
        }

        public async Task<ApiResult<BookingResponseDTO>> CompleteServiceAsync(CompleteServiceRequestDTO request, Guid actorId)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(request.BookingId, trackChanges: true);
            if (booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Không tìm thấy thông tin đặt lịch");
            }
            if (booking.Status != BookingStatus.InProgress)
            {
                return new ApiErrorResult<BookingResponseDTO>($"Chỉ có thể hoàn thành dịch vụ khi đơn đang ở trạng thái 'InProgress'. Trạng thái hiện tại: '{booking.Status}'.");
            }
            var procedures = await _unitOfWork.BookingProcedureRepository.GetProceduresByBookingIdAsync(request.BookingId);

            if (procedures.Any())
            {
                var incompleteRequiredProcedures = procedures.Where(x =>
                    x.IsRequired &&
                    x.Status != BookingProcedureStatus.Completed &&
                    x.Status != BookingProcedureStatus.Skipped)
                    .ToList();
                if (incompleteRequiredProcedures.Any())
                {
                    var names = string.Join(", ", incompleteRequiredProcedures.Select(p => p.ProcedureName));
                    return new ApiErrorResult<BookingResponseDTO>(
                                    $"Không thể hoàn thành dịch vụ. Các bước bắt buộc sau chưa hoàn thành: {names}.");
                }
            }
            string finalUrls = string.Join(",", request.CompleteImagesUrl);
            booking.CompleteService(finalUrls, actorId);

            _unitOfWork.BookingRepository.Update(booking);
            await _unitOfWork.SaveChangesAsync();

            var response = _mapper.Map<BookingResponseDTO>(booking);
            return new ApiSuccessResult<BookingResponseDTO>(response, "Hoàn thành dịch vụ làm móng thành công");
        }

        public async Task<ApiResult<BookingResponseDTO>> ReceptionistAssignArtistAsync(Guid bookingId, AssignArtistRequestDTO request, Guid actorId)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(bookingId, trackChanges: true);
            if (booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Không tìm thấy thông tin đặt lịch.");
            }

            if (booking.Status != BookingStatus.Pending && booking.Status != BookingStatus.Approved && booking.Status != BookingStatus.CheckedIn)
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
                    artist.ConcurrentCapacity
                );
                if (isConflict)
                {
                    return new ApiErrorResult<BookingResponseDTO>($"Thợ {artist.Account.FirstName} đã bị trùng hoặc quá tải lịch làm việc trong khung giờ này.");
                }

                // Cập nhật gán thợ và timeline cho các bước con
                foreach (var segment in timeline)
                {
                    var proc = procedures.First(x => x.BookingProcedureId == segment.BookingProcedureId);
                    proc.EstimatedStartTime = segment.StartTime;
                    proc.EstimatedEndTime = segment.EndTime;
                    if (proc.ActiveDuration > 0)
                    {
                        proc.AssignedArtistId = request.StaffArtistId;
                    }
                    _unitOfWork.BookingProcedureRepository.Update(proc);
                }
            }
            // Cập nhật thợ nail
            booking.ReceptionistAssignArtist(request.StaffArtistId, $"{artist.Account.FirstName} {artist.Account.LastName}", actorId);

            _unitOfWork.BookingRepository.Update(booking);
            await _unitOfWork.SaveChangesAsync();

            var savedBooking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(booking.BookingId);
            var response = _mapper.Map<BookingResponseDTO>(savedBooking);
            return new ApiSuccessResult<BookingResponseDTO>(response, "Tiếp tân chỉ định thợ nail thành công.");
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
                                  approvedArtist.ConcurrentCapacity);
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
                var isConflict = await _bookingSchedulingService.HasCapacityConflictAsync(
                                         artist.NailArtistId,
                                         booking.BookingDate,
                                         timeline,
                                         artist.ConcurrentCapacity);
                if (isConflict) continue;
                availableArtists.Add(artist);
            }
            var response = _mapper.Map<List<SuggestedArtistResponseDTO>>(availableArtists);
            return new ApiSuccessResult<List<SuggestedArtistResponseDTO>>(response, "Lấy danh sách thợ rảnh cho đơn đặt lịch thành công.");
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
    }
}
