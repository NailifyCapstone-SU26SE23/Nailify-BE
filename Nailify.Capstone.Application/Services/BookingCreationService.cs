using AutoMapper;
using Microsoft.Extensions.Logging;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.Common.Helpers;
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

namespace Nailify.Capstone.Application.Services
{
    public class BookingCreationService : IBookingCreationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IQRService _qrService;
        private readonly IBookingProcedureService _bookingProcedureService;
        private readonly ILoyaltyTierService _loyaltyTierService;
        private readonly ISlotHoldService _slotHoldService;
        private readonly IPromotionService _promotionService;
        private readonly IBookingSchedulingService _bookingSchedulingService;
        private readonly INailVariantService _nailVariantService;
        private readonly ILogger<BookingCreationService> _logger;

        public BookingCreationService(
                                      IUnitOfWork unitOfWork,
                                      IMapper mapper,
                                      IQRService qrService,
                                      IBookingProcedureService bookingProcedureService,
                                      ILoyaltyTierService loyaltyTierService,
                                      ISlotHoldService slotHoldService,
                                      IPromotionService promotionService,
                                      IBookingSchedulingService bookingSchedulingService,
                                      INailVariantService nailVariantService,
                                      ILogger<BookingCreationService> logger
                                      )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _qrService = qrService;
            _bookingProcedureService = bookingProcedureService;
            _loyaltyTierService = loyaltyTierService;
            _slotHoldService = slotHoldService;
            _promotionService = promotionService;
            _bookingSchedulingService = bookingSchedulingService;
            _nailVariantService = nailVariantService;
            _logger = logger;
        }

        public async Task<ApiResult<BookingPriceResponseDTO>> CalculateBookingPriceAsync(Guid? customerId, IEnumerable<BookingItemRequestDTO> bookingItems, List<int>? selectedPromotionIds = null)
        {
            var normalizedItems = NormalizePriceRequestItems(bookingItems);
            var normalizedPromotionIds = selectedPromotionIds?
                .Where(id => id > 0)
                .Distinct()
                .ToList();

            if (!normalizedItems.Any())
            {
                return new ApiSuccessResult<BookingPriceResponseDTO>(
                    new BookingPriceResponseDTO(),
                    "Tính giá đặt lịch thành công.");
            }

            var bookingId = Guid.NewGuid();
            var calculation = await BuildBookingItemsAsync(
                normalizedItems,
                bookingId,
                salonId: null);

            if (!calculation.IsSucceeded)
            {
                return new ApiErrorResult<BookingPriceResponseDTO>(calculation.ErrorMessage!);
            }

            var promotionDiscountAmount = 0m;
            var loyaltyDiscountAmount = 0m;
            var appliedPromotionDiscounts = new List<BookingDiscount>();

            if (customerId.HasValue)
            {
                var loyaltyResult = await _loyaltyTierService.GetMyLoyaltyAsync(customerId.Value);
                if (!loyaltyResult.IsSucceeded)
                {
                    return new ApiErrorResult<BookingPriceResponseDTO>(loyaltyResult.Message);
                }

                var applicablePromotions = await _promotionService.GetApplicablePromotionsAsync(
                    customerId.Value,
                    calculation.Items,
                    normalizedPromotionIds);

                (promotionDiscountAmount, appliedPromotionDiscounts) =
                    await _promotionService.CalculateDiscountsAsync(new Booking
                    {
                        BookingId = bookingId,
                        CustomerId = customerId.Value,
                        BookingItems = calculation.Items
                    }, applicablePromotions);

                loyaltyDiscountAmount = decimal.Round(
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
                        AppliedDate = DateTime.UtcNow.AddHours(7),
                        LoyaltyTierId = loyaltyResult.Data.LoyaltyTier.LoyaltyTierId
                    });
                }
            }

            var discountBreakdown = appliedPromotionDiscounts
                .Select(discount => new DiscountBreakdownDTO
                {
                    Name = discount.Name,
                    Amount = discount.DiscountAmount,
                    Type = discount.LoyaltyTierId.HasValue ? "Loyalty" : "Promotion"
                })
                .ToList();

            var totalDiscountAmount = loyaltyDiscountAmount + promotionDiscountAmount;
            var response = new BookingPriceResponseDTO
            {
                Price = calculation.Price,
                Discount = -totalDiscountAmount,
                TotalPrice = Math.Max(0, calculation.Price - totalDiscountAmount),
                TotalDuration = calculation.Duration,
                DiscountBreakdown = discountBreakdown
            };

            return new ApiSuccessResult<BookingPriceResponseDTO>(response, "Tính giá đặt lịch thành công.");
        }

        private static List<BookingItemRequestDTO> NormalizePriceRequestItems(IEnumerable<BookingItemRequestDTO>? bookingItems)
        {
            return bookingItems?
                .Select(item => new BookingItemRequestDTO
                {
                    NailVariantId = NormalizeNullableId(item.NailVariantId),
                    ServiceId = NormalizeNullableGuid(item.ServiceId),
                    ShapeMethodConfigId = NormalizeNullableId(item.ShapeMethodConfigId),
                    CustomerNailId = NormalizeNullableId(item.CustomerNailId),
                    CustomerNailRequestId = NormalizeNullableGuid(item.CustomerNailRequestId),
                    Quantity = Math.Max(item.Quantity, 1)
                })
                .Where(item =>
                    item.NailVariantId.HasValue
                    || item.ServiceId.HasValue
                    || item.ShapeMethodConfigId.HasValue
                    || item.CustomerNailId.HasValue
                    || item.CustomerNailRequestId.HasValue)
                .ToList() ?? new List<BookingItemRequestDTO>();
        }

        private static int? NormalizeNullableId(int? id)
            => id.HasValue && id.Value > 0 ? id.Value : null;

        private static Guid? NormalizeNullableGuid(Guid? id)
            => id.HasValue && id.Value != Guid.Empty && id.Value != SampleSwaggerGuid ? id.Value : null;

        private static readonly Guid SampleSwaggerGuid = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");

        public async Task<ApiResult<BookingResponseDTO>> CreateBookingAsync(Guid customerId, CreateBookingRequestDTO request)
        {
            if (request.BookingItems == null || !request.BookingItems.Any())
            {
                return new ApiErrorResult<BookingResponseDTO>("Vui lòng chọn ít nhất một mẫu móng hoặc dịch vụ.");
            }

            // BR-01: Ngày đặt lịch không được là ngày trong quá khứ
            var localToday = DateTime.UtcNow.AddHours(7).Date;
            var requestLocalDate = (request.BookingDate.Kind == DateTimeKind.Utc ? request.BookingDate.AddHours(7) : request.BookingDate).Date;
            if (requestLocalDate < localToday)
            {
                return new ApiErrorResult<BookingResponseDTO>("Ngày đặt lịch không được là ngày trong quá khứ.");
            }

            // Tự động kiểm tra và cưỡng chế thợ khi đặt lịch mẫu custom
            var customRequestError = await ResolveCustomerNailRequestIdsAsync(request.BookingItems, request.SalonId, request.NailArtistId);
            if (!string.IsNullOrWhiteSpace(customRequestError))
            {
                return new ApiErrorResult<BookingResponseDTO>(customRequestError);
            }

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
            var localDate = (request.BookingDate.Kind == DateTimeKind.Utc ? request.BookingDate.AddHours(7) : request.BookingDate).Date;
            var isOffDay = await _unitOfWork.SalonOffDateRepository.ExistsAsync(x =>
                                                         x.SalonId == request.SalonId
                                                         && x.StartDate.Date <= localDate
                                                         && x.EndDate.Date >= localDate);
            if (isOffDay)
            {
                return new ApiErrorResult<BookingResponseDTO>("Chi nhánh đóng cửa nghỉ lễ vào ngày này.");
            }
            var bookingId = Guid.NewGuid();
            var calculation = await BuildBookingItemsAsync(
                request.BookingItems,
                bookingId,
                request.SalonId,
                request.NailArtistId,
                createMissingCustomerNailRequest: true);
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
                    AppliedDate = DateTime.UtcNow.AddHours(7),
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
            if (request.WarrantyForBookingId.HasValue)
            {
                var oldBooking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(request.WarrantyForBookingId.Value);
                if (oldBooking == null)
                {
                    return new ApiErrorResult<BookingResponseDTO>("Không tìm thấy đơn hàng gốc cần bảo hành.");
                }
                if (oldBooking.CustomerId != customerId)
                {
                    return new ApiErrorResult<BookingResponseDTO>("Đơn hàng gốc không thuộc về tài khoản này.");
                }
                if (oldBooking.Status != BookingStatus.Completed)
                {
                    return new ApiErrorResult<BookingResponseDTO>("Đơn hàng gốc chưa hoàn thành để bảo hành.");
                }
                var completedDate = oldBooking.UpdatedAt; // Ngày hoàn thành
                if (completedDate.HasValue && DateTime.UtcNow.AddHours(7).Date > completedDate.Value.Date.AddDays(7))
                {
                    return new ApiErrorResult<BookingResponseDTO>("Đơn hàng gốc đã quá hạn bảo hành (Hạn bảo hành là 7 ngày).");
                }
                var isAlreadyWarranted = await _unitOfWork.BookingRepository.ExistsAsync(x =>
                                                                x.WarrantyForBookingId == request.WarrantyForBookingId.Value
                                                                && x.Status != BookingStatus.Cancelled
                                                                && x.Status != BookingStatus.Rejected);
                if (isAlreadyWarranted)
                {
                    return new ApiErrorResult<BookingResponseDTO>("Đơn đặt lịch gốc này đã được yêu cầu bảo hành trước đó.");
                }
                foreach (var item in request.BookingItems)
                {
                    bool isValidItem = oldBooking.BookingItems.Any(oldItem =>
                                                                              (item.NailVariantId.HasValue && oldItem.NailVariantId == item.NailVariantId) ||
                                                                              (item.ServiceId.HasValue && oldItem.ServiceId == item.ServiceId) ||
                                                                              (item.CustomerNailId.HasValue && oldItem.CustomerNailRequest != null && oldItem.CustomerNailRequest.CustomerNailId == item.CustomerNailId)
                                                                   );
                    if (!isValidItem)
                    {
                        return new ApiErrorResult<BookingResponseDTO>("Dịch vụ hoặc mẫu móng yêu cầu bảo hành không khớp với đơn đặt lịch gốc.");
                    }
                }
                bookingPrice.Price = 0;
                bookingPrice.Discount = 0;
                bookingPrice.TotalPrice = 0;
            }
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
            booking.AmountDue = bookingPrice.TotalPrice;

            // BẮT ĐẦU TRANSACTION AN TOÀN TRÁNH RACE CONDITION KHI TẠO BOOKING
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var dayOfWeek = (int)localDate.DayOfWeek;

                var salon = await _unitOfWork.SalonRepository.GetSalonWithOperatingHoursAsync(request.SalonId);
                var operatingHours = salon?.OperatingHours?.Where(x => x.DayOfWeek == dayOfWeek).ToList() ?? new List<SalonOperatingHour>();
                var targetEndTime = request.StartTime.Add(TimeSpan.FromMinutes(totalDuration));
                if (!operatingHours.IsWithinOperatingHours(request.StartTime, targetEndTime))
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return new ApiErrorResult<BookingResponseDTO>("Thời gian đặt lịch không nằm trong giờ hoạt động của Salon.");
                }
                if (request.NailArtistId.HasValue)
                {
                    var artistBreaks = await _unitOfWork.NailArtistBreakRepository.GetApprovedBreaksByArtistAndDateAsync(request.NailArtistId.Value, request.BookingDate);
                    bool overlapsBreak = artistBreaks.Any(b => request.StartTime < b.EndTime && targetEndTime > b.StartTime);
                    if (overlapsBreak)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        return new ApiErrorResult<BookingResponseDTO>("Thợ nail đã đăng ký nghỉ trong khung giờ này.");
                    }
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
                            if (procedure.ActiveDuration > 0 && procedure.IsMainStep)
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

        private async Task<string?> ResolveCustomerNailRequestIdsAsync(
          IEnumerable<BookingItemRequestDTO> bookingItems,
          Guid salonId,
          Guid? staffId,
          bool createMissingRequest = true)
        {
            foreach (var item in bookingItems)
            {
                if (!item.CustomerNailId.HasValue || item.CustomerNailRequestId.HasValue)
                {
                    continue;
                }

                var customerNail = await _unitOfWork.CustomerNailRepository.GetCustomerNailDetailAsync(item.CustomerNailId.Value);
                if (customerNail == null)
                {
                    return $"Không tìm thấy nail custom ID {item.CustomerNailId.Value}";
                }

                var customerNailRequest = await _unitOfWork.CustomerNailRequestRepository
                    .GetByCustomerNailAndSalonAsync(item.CustomerNailId.Value, salonId);

                if (customerNailRequest == null && !createMissingRequest)
                {
                    return $"Không tìm thấy nail custom ID {item.CustomerNailId.Value}.";
                }

                if (customerNailRequest == null)
                {
                    customerNailRequest = new CustomerNailRequest
                    {
                        CustomerNailRequestId = Guid.NewGuid(),
                        CustomerNailId = item.CustomerNailId.Value,
                        SalonId = salonId,
                        Status = CustomerNailStatus.Quoted,
                        ApprovedArtistId = staffId,
                        Price = null,
                        Duration = null,
                        CreatedAt = DateTime.UtcNow.AddHours(7)
                    };

                    await _unitOfWork.CustomerNailRequestRepository.CreateAsync(customerNailRequest);
                }

                item.CustomerNailRequestId = customerNailRequest.CustomerNailRequestId;
            }

            return null;
        }

        private async Task<BookingItemsCalculation> BuildBookingItemsAsync(
         IEnumerable<BookingItemRequestDTO> requests,
         Guid bookingId,
         Guid? salonId,
         Guid? customerPassedArtistId = null,
         bool createMissingCustomerNailRequest = false)
        {
            var requestItems = requests?.ToList() ?? new List<BookingItemRequestDTO>();
            if (!requestItems.Any())
            {
                return BookingItemsCalculation.Failure("Vui lòng chọn ít nhất một mẫu móng hoặc dịch vụ.");
            }

            if (salonId.HasValue && salonId.Value != Guid.Empty)
            {
                var customRequestError = await ResolveCustomerNailRequestIdsAsync(
                    requestItems,
                    salonId.Value,
                    customerPassedArtistId,
                    createMissingCustomerNailRequest);
                if (!string.IsNullOrWhiteSpace(customRequestError))
                {
                    return BookingItemsCalculation.Failure(customRequestError);
                }
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

                    unitPrice += customNailRequest.Price ?? customNail.Price ?? 0;
                    unitDuration += customNailRequest.Duration ?? customNail.Duration ?? 60;
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
                                "Cấu hình cách làm không thuộc dáng móng đã chọn.");
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
                                "Cấu hình cách làm không thuộc dáng móng đã chọn.");
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
                item.Duration = unitDuration;
                totalPrice += unitPrice * item.Quantity;
                totalDuration += unitDuration * item.Quantity;
            }

            return BookingItemsCalculation.Success(items, totalPrice, totalDuration);
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
    }
}
