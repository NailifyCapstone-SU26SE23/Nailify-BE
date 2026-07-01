using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs;
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
        public BookingService(IUnitOfWork unitOfWork, IMapper mapper, IQRService qrService, IBookingProcedureService bookingProcedureService, INailVariantService nailVariantService, ISlotHoldService slotHoldService, ILoyaltyTierService loyaltyTierService, IPromotionService promotionService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _qrService = qrService;
            _bookingProcedureService = bookingProcedureService;
            _nailVariantService = nailVariantService;
            _loyaltyTierService = loyaltyTierService;
            _slotHoldService = slotHoldService;
            _promotionService = promotionService;
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
            var response = _mapper.Map<BookingResponseDTO>(booking);
            return new ApiSuccessResult<BookingResponseDTO>(response, "Khách hàng Check-in thành công.");
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
            var customNailItem = request.BookingItems.FirstOrDefault(x => x.CustomerNailId.HasValue);
            if (customNailItem != null)
            {
                var customNail = await _unitOfWork.CustomerNailRepository.GetCustomerNailDetailAsync(customNailItem.CustomerNailId!.Value);
                if (customNail == null)
                {
                    return new ApiErrorResult<BookingResponseDTO>($"Không tìm thấy mẫu móng custom ID {customNailItem.CustomerNailId.Value}");
                }

                // Tìm bản ghi CustomerNailRequest đã được Approved tại salon này
                var customNailRequest = await _unitOfWork.CustomerNailRequestRepository.GetApprovedRequestAsync(customNailItem.CustomerNailId.Value, request.SalonId);
                if (customNailRequest == null)
                {
                    return new ApiErrorResult<BookingResponseDTO>($"Mẫu móng custom '{customNail.Name}' chưa được duyệt báo giá hoặc đã bị từ chối tại chi nhánh này.");
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

            if (request.NailArtistId.HasValue)
            {
                if (!string.IsNullOrEmpty(request.HoldToken))
                {
                    var isValid = await _slotHoldService.ValidateHoldTokenAsync(request.HoldToken, customerId, request.NailArtistId.Value, request.BookingDate, request.StartTime);
                    if (!isValid)
                    {
                        return new ApiErrorResult<BookingResponseDTO>("Mã giữ chỗ không hợp lệ hoặc đã hết hạn.");
                    }
                }
                else
                {
                    var targetEnd = request.StartTime.Add(TimeSpan.FromMinutes(totalDuration));
                    var isHeld = await _slotHoldService.IsSlotHeldAsync(request.NailArtistId.Value, request.BookingDate, request.StartTime, targetEnd);
                    if (isHeld)
                    {
                        return new ApiErrorResult<BookingResponseDTO>("Khoảng thời gian này đang có người giữ chỗ. Vui lòng thử lại sau hoặc sử dụng mã giữ chỗ nếu bạn đã có.");
                    }
                }
                var targetEndTime = request.StartTime.Add(TimeSpan.FromMinutes(totalDuration));
                var isConflict = await _unitOfWork.BookingRepository.HasBookingConflictAsync(
                    request.NailArtistId.Value,
                    request.BookingDate,
                    request.StartTime,
                    targetEndTime
                );
                if (isConflict)
                {
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
            if (!string.IsNullOrEmpty(request.HoldToken))
            {
                await _slotHoldService.ConsumeHoldAsync(request.HoldToken);
            }
            foreach (var item in booking.BookingItems)
            {
                if (item.NailVariantId.HasValue)
                {
                    await _bookingProcedureService.DuplicateProceduresForBookingItemAsync(item.BookingItemId, item.NailVariantId.Value);
                }
            }
            await _unitOfWork.SaveChangesAsync();
            var savedBooking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(booking.BookingId);
            var response = _mapper.Map<BookingResponseDTO>(savedBooking);
            return new ApiSuccessResult<BookingResponseDTO>(response, "Tạo đơn đặt lịch thành công.");
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

            var bookings = await _unitOfWork.BookingRepository.GetBookingsByArtistAndDateAsync(request.NailArtistId, request.BookingDate);

            var busySlots = bookings.Select(x => new BusyTimeSlotResponseDto
            {
                StartTime = x.StartTime,
                EndTime = x.StartTime.Add(TimeSpan.FromMinutes(x.TotalDuration))
            })
            .OrderBy(x => x.StartTime)
            .ToList();

            var timeSlots = new List<TimeSlotResponseDTO>();
            var currentStart = schedule.ShiftStart;
            var slotInterval = TimeSpan.FromMinutes(30);

            while (currentStart + slotInterval <= schedule.ShiftEnd)
            {
                var currentEnd = currentStart + slotInterval;
                bool isAvailable = !busySlots.Any(busy => currentStart < busy.EndTime && currentEnd > busy.StartTime);
                bool isHeld = false;
                if (isAvailable)
                {
                    // Kiểm tra xem slot có bị ai giữ tạm thời trên Redis không
                    isHeld = await _slotHoldService.IsSlotHeldAsync(request.NailArtistId, request.BookingDate, currentStart, currentEnd);
                }

                timeSlots.Add(new TimeSlotResponseDTO
                {
                    StartTime = currentStart,
                    EndTime = currentEnd,
                    IsAvailable = isAvailable && !isHeld,
                    IsHeld = isHeld
                });

                currentStart = currentEnd;
            }

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
            if (bookingItems.Any(item => !item.NailVariantId.HasValue && !item.ServiceId.HasValue && !item.CustomerNailId.HasValue))
            {
                return new ApiErrorResult<List<SuggestedArtistResponseDTO>>("Mỗi mục đặt lịch phải chứa ít nhất một dịch vụ, một mẫu nail hoặc một mẫu custom.");
            }

            // Custom: Nếu là đặt lịch mẫu custom, chỉ hiển thị thợ đã duyệt báo giá mẫu này
            var customNailItem = bookingItems.FirstOrDefault(x => x.CustomerNailId.HasValue);
            if (customNailItem != null)
            {
                var customNailRequest = await _unitOfWork.CustomerNailRequestRepository.GetApprovedRequestAsync(customNailItem.CustomerNailId.Value, request.SalonId);
                if (customNailRequest != null && customNailRequest.ApprovedArtistId.HasValue)
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
            if (bookingItems.Any(item => !item.NailVariantId.HasValue && !item.ServiceId.HasValue && !item.CustomerNailId.HasValue))
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

                if (item.CustomerNailId.HasValue)
                {
                    var customNailRequest = await _unitOfWork.CustomerNailRequestRepository.GetApprovedRequestAsync(item.CustomerNailId.Value, request.SalonId);
                    if (customNailRequest != null && customNailRequest.Duration.HasValue)
                    {
                        totalDuration += customNailRequest.Duration.Value;
                    }
                    else
                    {
                        var customNail = await _unitOfWork.CustomerNailRepository.GetCustomerNailDetailAsync(item.CustomerNailId.Value);
                        if (customNail != null)
                        {
                            totalDuration += (customNail.Duration ?? 60);
                        }
                    }
                }
            }

            IEnumerable<NailArtist> qualifiedArtists;
            var customNailItem = bookingItems.FirstOrDefault(x => x.CustomerNailId.HasValue);

            if (customNailItem != null)
            {
                var customNailRequest = await _unitOfWork.CustomerNailRequestRepository.GetApprovedRequestAsync(customNailItem.CustomerNailId.Value, request.SalonId);
                if (customNailRequest != null && customNailRequest.ApprovedArtistId.HasValue)
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

                var isConflict = await _unitOfWork.BookingRepository.HasBookingConflictAsync(
                    artist.NailArtistId,
                    request.BookingDate,
                    request.StartTime,
                    targetEndTime
                );
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

            decimal oldPrice = (decimal)booking.TotalPrice;
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
                    NailVariantId = x.NailVariantId
                };

                if (!x.NailVariantId.HasValue && !x.ServiceId.HasValue)
                {
                    return new ApiErrorResult<BookingResponseDTO>("Mỗi mục đặt lịch phải chứa ít nhất một dịch vụ hoặc một mẫu nail.");
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

                item.Price = itemPrice;
                item.DiscountAmount = 0;
                item.FinalPrice = itemPrice * Math.Max(item.Quantity, 1);
                item.Duration = itemDuration;

                totalDuration += item.Duration;
                totalPrice += item.Price;

                bookingItems.Add(item);
            }

            if (request.NailArtistId.HasValue)
            {
                var targetEndTime = request.StartTime.Add(TimeSpan.FromMinutes(totalDuration));
                var isConflict = await _unitOfWork.BookingRepository.HasBookingConflictExcludingCurrentAsync(
                    request.NailArtistId.Value,
                    request.BookingDate,
                    request.StartTime,
                    targetEndTime,
                    bookingId
                );
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
                oldItem.CustomerNail = null;
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
            await _unitOfWork.SaveChangesAsync();
            var savedBooking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(booking.BookingId);
            var response = _mapper.Map<BookingResponseDTO>(savedBooking);
            return new ApiSuccessResult<BookingResponseDTO>(response, "Duyệt đơn đặt lịch thành công.");
        }

        public async Task<ApiResult<BookingResponseDTO>> ManualCheckInBookingAsync(Guid bookingId, Guid actorId)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(bookingId, trackChanges: true);
            if (booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Đơn đặt lịch không tồn tại.");
            }

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

                if (!item.NailVariantId.HasValue && !item.ServiceId.HasValue && !item.CustomerNailId.HasValue)
                {
                    return BookingItemsCalculation.Failure(
                        "Mỗi mục đặt lịch phải chứa ít nhất một dịch vụ, một mẫu nail hoặc một mẫu custom.");
                }

                decimal unitPrice = 0;
                var unitDuration = 0;

                if (item.CustomerNailId.HasValue)
                {
                    var customNail = await _unitOfWork.CustomerNailRepository.GetCustomerNailDetailAsync(item.CustomerNailId.Value);
                    if (customNail == null)
                    {
                        return BookingItemsCalculation.Failure(
                            $"Không tìm thấy mẫu móng custom ID {item.CustomerNailId.Value}");
                    }

                    CustomerNailRequest? customNailRequest = null;
                    if (salonId.HasValue && salonId.Value != Guid.Empty)
                    {
                        customNailRequest = await _unitOfWork.CustomerNailRequestRepository.GetApprovedRequestAsync(item.CustomerNailId.Value, salonId.Value);
                    }
                    else
                    {
                        customNailRequest = await _unitOfWork.CustomerNailRequestRepository.GetAnyApprovedRequestAsync(item.CustomerNailId.Value);
                    }

                    if (customNailRequest == null)
                    {
                        return BookingItemsCalculation.Failure(
                            $"Mẫu móng custom '{customNail.Name}' chưa được duyệt báo giá hoặc đã bị từ chối.");
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

                    unitPrice += customNailRequest.Price ?? 0;
                    unitDuration += customNailRequest.Duration ?? 60;
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

            // Custom: Chỉ cho phép thợ duyệt mẫu custom nhận làm
            var customNailItem = bookingItems.FirstOrDefault(x => x.CustomerNailId.HasValue);
            if (customNailItem != null)
            {
                var customNail = await _unitOfWork.CustomerNailRepository.GetCustomerNailDetailAsync(customNailItem.CustomerNailId!.Value);
                if (customNail != null)
                {
                    var customNailRequest = await _unitOfWork.CustomerNailRequestRepository.GetApprovedRequestAsync(customNailItem.CustomerNailId.Value, booking.SalonId);
                    if (customNailRequest != null && customNailRequest.ApprovedArtistId.HasValue)
                    {
                        var approvedArtist = await _unitOfWork.NailArtistRepository.GetNailArtistWithProfileAsync(customNailRequest.ApprovedArtistId.Value);
                        if (approvedArtist != null && approvedArtist.Status == "Active")
                        {
                            var schedule = await _unitOfWork.ScheduleRepository.GetScheduleByArtistAndDateAsync(approvedArtist.NailArtistId, booking.BookingDate);
                            if (schedule != null && booking.StartTime >= schedule.ShiftStart && targetEndTime <= schedule.ShiftEnd)
                            {
                                var isConflict = await _unitOfWork.BookingRepository.HasBookingConflictAsync(
                                                                                                             approvedArtist.NailArtistId,
                                                                                                             booking.BookingDate,
                                                                                                             booking.StartTime,
                                                                                                             targetEndTime);
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
                var isConflict = await _unitOfWork.BookingRepository.HasBookingConflictAsync(
                    artist.NailArtistId,
                    booking.BookingDate,
                    booking.StartTime,
                    targetEndTime
                );
                if (isConflict) continue;
                availableArtists.Add(artist);
            }
            var response = _mapper.Map<List<SuggestedArtistResponseDTO>>(availableArtists);
            return new ApiSuccessResult<List<SuggestedArtistResponseDTO>>(response, "Lấy danh sách thợ rảnh cho đơn đặt lịch thành công.");
        }
    }
}
