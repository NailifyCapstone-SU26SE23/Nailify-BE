using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs;
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
        private readonly IBookingCreationService _bookingCreationService;
        public BookingWaitlistService(
                                        IUnitOfWork unitOfWork,
                                        IMapper mapper,
                                        ILoyaltyTierService loyaltyTierService,
                                        IPromotionService promotionService,
                                        IBookingProcedureService bookingProcedureService,
                                        IBookingSchedulingService bookingSchedulingService,
                                        IBookingCreationService bookingCreationService
                                     )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _loyaltyTierService = loyaltyTierService;
            _promotionService = promotionService;
           _bookingProcedureService = bookingProcedureService;
            _bookingSchedulingService = bookingSchedulingService;
            _bookingCreationService = bookingCreationService;
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
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                //var wailist = await _unitOfWork.BookingWaitlistRepository.GetByIdAsync(waitlistId);
                var waitlist = await _unitOfWork.BookingWaitlistRepository.GetWaitlistForUpdateAsync(waitlistId);
                if (waitlist == null || waitlist.CustomerId != customerId)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return new ApiErrorResult<WaitlistResponseDTO>("Không tìm thấy thông tin hàng chờ hợp lệ.");
                }
                if (waitlist.Status != WaitlistStatus.Notified)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return new ApiErrorResult<WaitlistResponseDTO>("Lịch hẹn của bạn chưa được mở hoặc đã hết hiệu lực xác nhận.");
                }
                if (waitlist.ExpiresAt.HasValue && waitlist.ExpiresAt < DateTime.UtcNow)
                {
                    waitlist.Status = WaitlistStatus.Expired;
                    _unitOfWork.BookingWaitlistRepository.Update(waitlist);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync(); // Commit phần expire
                    return new ApiErrorResult<WaitlistResponseDTO>("Thời gian xác nhận giữ chỗ (15 phút) đã hết hạn.");
                }
                var bookingItemRequests = (request.BookingItems != null && request.BookingItems.Any())
                            ? request.BookingItems
                            : waitlist.WaitlistItems.Select(w => new BookingItemRequestDTO
                            {
                                Quantity = w.Quantity,
                                ServiceId = w.ServiceId,
                                NailVariantId = w.NailVariantId,
                                CustomerNailId = w.CustomerNailId,
                                ShapeMethodConfigId = w.ShapeMethodConfigId,
                                CustomerNailRequestId = w.CustomerNailRequestId
                            }).ToList();

                var createBookingRequest = new CreateBookingRequestDTO
                {
                    SalonId = waitlist.SalonId,
                    BookingDate = waitlist.RequestedDate,
                    StartTime = waitlist.RequestedStartTime,
                    NailArtistId = waitlist.PreferredNailArtistId,
                    BookingItems = bookingItemRequests,
                    SelectedPromotionIds = request.SelectedPromotionIds,
                    UseWalletBalance = request.UseWalletBalance,
                    HoldToken = null
                };
                var createResult = await _bookingCreationService.CreateBookingAsync(customerId, createBookingRequest);
                if (!createResult.IsSucceeded)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return new ApiErrorResult<WaitlistResponseDTO>($"Lỗi khi tạo lịch từ hàng chờ: {createResult.Message}");
                }
                // 6. Cập nhật trạng thái hàng chờ Waitlist
                waitlist.Status = WaitlistStatus.Confirmed;
                waitlist.ConvertedBookingId = createResult.Data.BookingId;
                
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
            // kiem tra xem co bao nhieu tho dang lam viec trong ngay do
            var workingArtistCount = await _unitOfWork.ScheduleRepository.GetWorkingArtistCountByDateAsync(request.SalonId, request.RequestedDate);

            if(workingArtistCount == 0)
            {
                return new ApiErrorResult<WaitlistResponseDTO>("Salon không có thợ làm việc vào ngày này.");
            }
            //  Tinh dung luong hang cho toi da cho khung gio
            int maxWailistCapcity = Math.Max(1, (int)Math.Ceiling(workingArtistCount * 0.3));

            // Dem luot cho dang co cho khung gio
            var activeWailistCount = await _unitOfWork.BookingWaitlistRepository.GetActiveWailistCountAsync(request.SalonId, request.RequestedDate, request.RequestedStartTime);

            if(activeWailistCount >= maxWailistCapcity)
            {
                return new ApiErrorResult<WaitlistResponseDTO>(
                                $"Hàng chờ cho khung giờ {request.RequestedStartTime:hh\\:mm} đã đạt giới hạn tối đa ({maxWailistCapcity} lượt). Vui lòng chọn khung giờ hoặc chi nhánh khác.");
            }

            int accumulatedWaitTime = activeWailistCount * 30;
            if(accumulatedWaitTime >= 60)
            {
                return new ApiErrorResult<WaitlistResponseDTO>(
                             $"Thời gian chờ dự kiến cho khung giờ này quá dài (~{accumulatedWaitTime} phút). Salon hiện tạm dừng nhận thêm lượt chờ.");
            }

            // Đếm số lượng lượt chờ hiện đang có cho khung giờ
            var position = await _unitOfWork.BookingWaitlistRepository.GetNextPositionAsync(
                request.SalonId, 
                request.RequestedDate, 
                request.RequestedStartTime, 
                request.PreferredNailArtistId);

            var wailist = _mapper.Map<BookingWaitlist>(request);
            wailist.CustomerId = customerId;
            wailist.Position = position;
            wailist.Status = WaitlistStatus.Waiting;
            wailist.CreatedAt = DateTime.UtcNow.AddHours(7);

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
