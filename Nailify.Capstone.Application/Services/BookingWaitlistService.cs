using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.WaitlistRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.WaitlistResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Common.Events.BookingEvents;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Services
{
    public class BookingWaitlistService : IBookingWaitlistService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILoyaltyTierService _loyaltyTierService;
        private readonly IPromotionService _promotionService;
        private readonly IBookingProcedureService _bookingProcedureService;
        private readonly IBookingSchedulingService _bookingSchedulingService;
        public BookingWaitlistService(
                                        IUnitOfWork unitOfWork,
                                        IMapper mapper,
                                        ILoyaltyTierService loyaltyTierService,
                                        IPromotionService promotionService,
                                        IBookingProcedureService bookingProcedureService,
                                        IBookingSchedulingService bookingSchedulingService
                                     )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _loyaltyTierService = loyaltyTierService;
            _promotionService = promotionService;
           _bookingProcedureService = bookingProcedureService;
            _bookingSchedulingService = bookingSchedulingService;
        }

        public async Task<ApiResult<WaitlistResponseDTO>> CancelWaitlistAsync(Guid waitlistId, Guid customerId)
        {
            var wailist = await _unitOfWork.BookingWaitlistRepository.GetByIdAsync(waitlistId);
            if(wailist == null || wailist.CustomerId != customerId)
            {
                return new ApiErrorResult<WaitlistResponseDTO>("Không tìm thấy thông tin hàng chờ.");
            }
            var previousStatus = wailist.Status;
            wailist.Status = WaitlistStatus.Cancelled;
            _unitOfWork.BookingWaitlistRepository.Update(wailist);
            if(previousStatus == WaitlistStatus.Notified)
            {
                var freedEvent = new SlotFreedEvent(wailist.SalonId, wailist.RequestedDate, wailist.RequestedStartTime);
                wailist.AddDomainEvent(freedEvent);
            }
            await _unitOfWork.SaveChangesAsync();
            var detailedWaitlist = await _unitOfWork.BookingWaitlistRepository.GetWaitlistWithDetailsAsync(waitlistId);
            var response = _mapper.Map<WaitlistResponseDTO>(detailedWaitlist ?? wailist);
            return new ApiSuccessResult<WaitlistResponseDTO>(response, "Hủy vị trí trong hàng chờ thành công.");
        }

        public async Task<ApiResult<WaitlistResponseDTO>> ConfirmWaitlistAsync(Guid waitlistId, Guid customerId, ConfirmWaitlistRequestDTO request)
        {
            //var wailist = await _unitOfWork.BookingWaitlistRepository.GetByIdAsync(waitlistId);
            var waitlist = await _unitOfWork.BookingWaitlistRepository.GetWaitlistWithItemsAsync(waitlistId);
            if (waitlist == null || waitlist.CustomerId != customerId)
            {
                return new ApiErrorResult<WaitlistResponseDTO>("Không tìm thấy thông tin hàng chờ hợp lệ.");
            }
            if (waitlist.Status != WaitlistStatus.Notified)
            {
                return new ApiErrorResult<WaitlistResponseDTO>("Lịch hẹn của bạn chưa được mở hoặc đã hết hiệu lực xác nhận.");
            }
            if (waitlist.ExpiresAt.HasValue && waitlist.ExpiresAt < DateTime.UtcNow)
            {
                waitlist.Status = WaitlistStatus.Expired;
                _unitOfWork.BookingWaitlistRepository.Update(waitlist);
                await _unitOfWork.SaveChangesAsync();
                return new ApiErrorResult<WaitlistResponseDTO>("Thời gian xác nhận giữ chỗ (15 phút) đã hết hạn.");
            }
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var bookingItems = new List<BookingItem>();
                decimal basePrice = 0;
                int totalDuration = 0;

                var sourceItems = (request?.BookingItems != null && request.BookingItems.Any())
                    ? request.BookingItems.Select(b => new { b.Quantity, b.ServiceId, b.NailVariantId, CustomerNailRequestId = b.CustomerNailRequestId, CustomerNailId = (int?)null })
                    : waitlist.WaitlistItems.Select(w => new { w.Quantity, w.ServiceId, w.NailVariantId, CustomerNailRequestId = (Guid?)null, w.CustomerNailId });

                foreach (var x in sourceItems)
                {
                    var item = new BookingItem
                    {
                        Quantity = x.Quantity,
                        ServiceId = x.ServiceId,
                        NailVariantId = x.NailVariantId,
                        CustomerNailRequestId = x.CustomerNailRequestId,
                    };
                    decimal itemPrice = 0;
                    int itemDuration = 0;
                    if (x.CustomerNailRequestId.HasValue)
                    {
                        var customNailRequest = await _unitOfWork.CustomerNailRequestRepository.GetByIdAsync(x.CustomerNailRequestId.Value);
                        if (customNailRequest != null)
                        {
                            item.CustomerNailRequestId = customNailRequest.CustomerNailRequestId;
                            itemPrice += customNailRequest.Price ?? 0;

                            var customNail = await _unitOfWork.CustomerNailRepository.GetCustomerNailDetailAsync(customNailRequest.CustomerNailId);
                            itemDuration += customNail?.Duration ?? 60;
                            if (customNailRequest.Duration.HasValue)
                            {
                                itemDuration += customNailRequest.Duration.Value;
                            }
                        }
                    }
                    else if (x.CustomerNailId.HasValue)
                    {

                        var customNailRequest = await _unitOfWork.CustomerNailRequestRepository.GetApprovedRequestAsync(x.CustomerNailId.Value, waitlist.SalonId);
                        if (customNailRequest != null)
                        {
                            item.CustomerNailRequestId = customNailRequest.CustomerNailRequestId;
                            itemPrice += customNailRequest.Price ?? 0;

                            var customNail = await _unitOfWork.CustomerNailRepository.GetCustomerNailDetailAsync(customNailRequest.CustomerNailId);
                            itemDuration += customNail?.Duration ?? 60;
                            if (customNailRequest.Duration.HasValue)
                            {
                                itemDuration += customNailRequest.Duration.Value;
                            }
                        }
                        else
                        {
                            var customNail = await _unitOfWork.CustomerNailRepository.GetCustomerNailDetailAsync(x.CustomerNailId.Value);
                            if (customNail != null)
                            {
                                itemDuration += customNail.Duration ?? 60;
                            }
                        }
                    }
                    if (x.NailVariantId.HasValue)
                    {
                        var variant = await _unitOfWork.NailVariantRepository.GetByIdAsync(x.NailVariantId.Value);
                        if (variant != null)
                        {
                            itemPrice += variant.Price;
                            itemDuration += (variant.Duration ?? 60);
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
                    }
                    item.Price = itemPrice;
                    item.Duration = itemDuration;
                    basePrice += item.Price * Math.Max(item.Quantity, 1);
                    totalDuration += item.Duration;
                    bookingItems.Add(item);
                }
               
                var loyaltyResult = await _loyaltyTierService.GetMyLoyaltyAsync(customerId);
                if (!loyaltyResult.IsSucceeded)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return new ApiErrorResult<WaitlistResponseDTO>(loyaltyResult.Message);
                }
                var dummyBooking = new Booking
                {
                    CustomerId = customerId,
                    BookingItems = bookingItems
                };
                var applicablePromotions = await _promotionService.GetApplicablePromotionsAsync(customerId, bookingItems, new List<int>());
                var (promotionDiscountAmount, appliedPromotionDiscounts) = await _promotionService.CalculateDiscountsAsync(dummyBooking, applicablePromotions);
                var loyaltyDiscountAmount = decimal.Round(
                    basePrice * loyaltyResult.Data.LoyaltyTier.DiscountRate,
                    0,
                    MidpointRounding.AwayFromZero
                );
                if (loyaltyDiscountAmount > 0)
                {
                    appliedPromotionDiscounts.Add(new BookingDiscount
                    {
                        Name = $"{loyaltyResult.Data.LoyaltyTier.Name} Tier",
                        DiscountAmount = loyaltyDiscountAmount,
                        IsAutoApplied = true,
                        AppliedDate = DateTime.UtcNow,
                        LoyaltyTierId = loyaltyResult.Data.LoyaltyTier.LoyaltyTierId
                    });
                }
                decimal totalDiscountAmount = loyaltyDiscountAmount + promotionDiscountAmount;
                decimal finalPrice = Math.Max(0, basePrice - totalDiscountAmount);
                // 3. Khởi tạo Booking mới
                var booking = new Booking
                {
                    CustomerId = customerId,
                    SalonId = waitlist.SalonId,
                    BookingDate = waitlist.RequestedDate,
                    StartTime = waitlist.RequestedStartTime,
                    NailArtistId = waitlist.PreferredNailArtistId,
                    Price = basePrice,
                    Discount = -totalDiscountAmount,
                    TotalPrice = finalPrice,
                    TotalDuration = totalDuration,
                    Status = BookingStatus.Pending,
                    BookingItems = bookingItems,
                    BookingDiscounts = appliedPromotionDiscounts
                };
                await _unitOfWork.BookingRepository.CreateAsync(booking);
                await _unitOfWork.SaveChangesAsync();
                foreach (var item in booking.BookingItems)
                {
                    await _bookingProcedureService.DuplicateProceduresForBookingItemAsync(item);
                }
                await _unitOfWork.SaveChangesAsync();

                // 5. Cập nhật và tính toán Timeline công đoạn ban đầu cho booking này
                var procedures = await _unitOfWork.BookingProcedureRepository.GetProceduresByBookingIdAsync(booking.BookingId, trackChanges: true);
                if (procedures.Any() && booking.NailArtistId.HasValue)
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
                }
                // 6. Cập nhật trạng thái hàng chờ Waitlist
                waitlist.Status = WaitlistStatus.Confirmed;
                waitlist.ConvertedBookingId = booking.BookingId;
                
                // Tránh tranh chấp tracking Entity Framework khi Update đối tượng NoTracking
                waitlist.Customer = null!;
                waitlist.PreferredNailArtist = null;
                waitlist.Salon = null!;
                
                _unitOfWork.BookingWaitlistRepository.Update(waitlist);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
                var detailedWaitlist = await _unitOfWork.BookingWaitlistRepository.GetWaitlistWithDetailsAsync(waitlistId);
                var response = _mapper.Map<WaitlistResponseDTO>(detailedWaitlist ?? waitlist);
                return new ApiSuccessResult<WaitlistResponseDTO>(response, "Xác nhận hàng chờ thành công. Lịch hẹn và quy trình chi tiết đã được lập!");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new ApiErrorResult<WaitlistResponseDTO>($"Lỗi hệ thống khi chuyển đổi lịch: {ex.Message}");
            }
        }
            

        public async Task<ApiResult<WaitlistResponseDTO>> GetMyWaitlistAsync(Guid customerId, Guid salonId)
        {
            var list = await _unitOfWork.BookingWaitlistRepository.GetActiveWaitlistByCustomerAsync(customerId, salonId);
            if (list == null)
            {
                return new ApiErrorResult<WaitlistResponseDTO>("Bạn không có lượt hàng chờ nào đang hoạt động.");
            }
            var response = _mapper.Map<WaitlistResponseDTO>(list);
            return new ApiSuccessResult<WaitlistResponseDTO>(response, "Lấy thông tin hàng chờ thành công.");
        }

        public async Task<ApiResult<List<WaitlistResponseDTO>>> GetMyWaitlistsAsync(Guid customerId)
        {
            var lists = await _unitOfWork.BookingWaitlistRepository.GetActiveWaitlistsByCustomerAsync(customerId);
            var response = _mapper.Map<List<WaitlistResponseDTO>>(lists);
            return new ApiSuccessResult<List<WaitlistResponseDTO>>(response, "Lấy danh sách hàng chờ thành công.");
        }

        public async Task<ApiResult<PagedList<WaitlistResponseDTO>>> GetSalonWaitlistAsync(Guid salonId, int pageNumber, int pageSize)
        {
            var paged = await _unitOfWork.BookingWaitlistRepository.GetSalonWaitlistWithDetailsAsync(salonId, pageNumber, pageSize);
            var dtos = paged.Items.Select(x => _mapper.Map<WaitlistResponseDTO>(x)).ToList();
            var response = new PagedList<WaitlistResponseDTO>(dtos, paged.MetaData.TotalItems, pageNumber, pageSize);
            return new ApiSuccessResult<PagedList<WaitlistResponseDTO>>(response, "Lấy danh sách hàng chờ salon thành công.");
        }

        public async Task<ApiResult<WaitlistResponseDTO>> JoinWaitlistAsync(Guid customerId, JoinWaitlistRequestDTO request)
        {
            // 1. Check duplicate waiting entry
            var isDuplicate = await _unitOfWork.BookingWaitlistRepository.IsDuplicateAsync(
                customerId, 
                request.SalonId, 
                request.RequestedDate, 
                request.RequestedStartTime, 
                request.PreferredNailArtistId);
            if (isDuplicate)
            {
                return new ApiErrorResult<WaitlistResponseDTO>("Bạn đã ở trong hàng chờ của khung giờ này rồi.");
            }
            var position = await _unitOfWork.BookingWaitlistRepository.GetNextPositionAsync(
                request.SalonId, 
                request.RequestedDate, 
                request.RequestedStartTime, 
                request.PreferredNailArtistId);

            var wailist = _mapper.Map<BookingWaitlist>(request);
            wailist.CustomerId = customerId;
            wailist.Position = position;
            wailist.Status = WaitlistStatus.Waiting;
            wailist.CreatedAt = DateTime.UtcNow;

            // Calculate EstimatedDuration based on WaitlistItems
            int totalDuration = 0;
            if (request.WaitlistItems != null)
            {
                foreach (var item in request.WaitlistItems)
                {
                    int itemDuration = 0;
                    if (item.CustomerNailId.HasValue)
                    {
                        var customNailRequest = await _unitOfWork.CustomerNailRequestRepository.GetApprovedRequestAsync(item.CustomerNailId.Value, request.SalonId);
                        if (customNailRequest != null)
                        {
                            var customNail = await _unitOfWork.CustomerNailRepository.GetCustomerNailDetailAsync(customNailRequest.CustomerNailId);
                            itemDuration += customNail?.Duration ?? 60;
                            if (customNailRequest.Duration.HasValue)
                            {
                                itemDuration += customNailRequest.Duration.Value;
                            }
                        }
                        else
                        {
                            var customNail = await _unitOfWork.CustomerNailRepository.GetCustomerNailDetailAsync(item.CustomerNailId.Value);
                            if (customNail != null)
                            {
                                itemDuration += customNail.Duration ?? 60;
                            }
                        }
                    }
                    if (item.NailVariantId.HasValue)
                    {
                        var variant = await _unitOfWork.NailVariantRepository.GetByIdAsync(item.NailVariantId.Value);
                        if (variant != null)
                        {
                            itemDuration += (variant.Duration ?? 60);
                        }
                    }
                    if (item.ServiceId.HasValue)
                    {
                        var service = await _unitOfWork.ServicesRepository.GetByIdAsync(item.ServiceId.Value);
                        if (service != null)
                        {
                            itemDuration += service.Duration;
                        }
                    }
                    totalDuration += itemDuration * Math.Max(item.Quantity, 1);
                }
            }
            wailist.EstimatedDuration = totalDuration;

            await _unitOfWork.BookingWaitlistRepository.CreateAsync(wailist);
            await _unitOfWork.SaveChangesAsync();
            var detailedWaitlist = await _unitOfWork.BookingWaitlistRepository.GetWaitlistWithDetailsAsync(wailist.WailistId);
            var response = _mapper.Map<WaitlistResponseDTO>(detailedWaitlist ?? wailist);
            return new ApiSuccessResult<WaitlistResponseDTO>(response, "Đăng ký vào hàng chờ thành công.");
        }
    }
}
