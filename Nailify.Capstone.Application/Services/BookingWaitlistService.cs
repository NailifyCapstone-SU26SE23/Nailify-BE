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

        public BookingWaitlistService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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

        public async Task<ApiResult<WaitlistResponseDTO>> ConfirmWaitlistAsync(Guid waitlistId, Guid customerId)
        {
            var wailist = await _unitOfWork.BookingWaitlistRepository.GetByIdAsync(waitlistId);
            if (wailist == null || wailist.CustomerId != customerId)
            {
                return new ApiErrorResult<WaitlistResponseDTO>("Không tìm thấy thông tin hàng chờ hợp lệ.");
            }
            if (wailist.Status != WaitlistStatus.Notified)
            {
                return new ApiErrorResult<WaitlistResponseDTO>("Lịch hẹn của bạn chưa được mở hoặc đã hết hiệu lực xác nhận.");
            }
            if (wailist.ExpiresAt.HasValue && wailist.ExpiresAt < DateTime.UtcNow)
            {
                wailist.Status = WaitlistStatus.Expired;
                _unitOfWork.BookingWaitlistRepository.Update(wailist);
                await _unitOfWork.SaveChangesAsync();
                return new ApiErrorResult<WaitlistResponseDTO>("Thời gian xác nhận giữ chỗ (15 phút) đã hết hạn.");
            }
            var booking = new Booking
            {
                CustomerId = customerId,
                SalonId = wailist.SalonId,
                BookingDate = wailist.RequestedDate,
                StartTime = wailist.RequestedStartTime,
                NailArtistId = wailist.PreferredNailArtistId ?? Guid.Empty,
                Status = BookingStatus.Approved,
            };
            await _unitOfWork.BookingRepository.CreateAsync(booking);
            wailist.Status = WaitlistStatus.Confirmed;
            wailist.ConvertedBookingId = booking.BookingId;
            _unitOfWork.BookingWaitlistRepository.Update(wailist);
            await _unitOfWork.SaveChangesAsync();
            var detailedWaitlist = await _unitOfWork.BookingWaitlistRepository.GetWaitlistWithDetailsAsync(waitlistId);
            var response = _mapper.Map<WaitlistResponseDTO>(detailedWaitlist ?? wailist);
            return new ApiSuccessResult<WaitlistResponseDTO>(response, "Xác nhận hàng chờ thành công. Lịch hẹn chính thức đã được tạo!");
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
            var isDuplicate = await _unitOfWork.BookingWaitlistRepository.IsDuplicateAsync(customerId, request.SalonId, request.RequestedDate, request.RequestedStartTime, request.PreferredNailArtistId);
            if (isDuplicate)
            {
                return new ApiErrorResult<WaitlistResponseDTO>("Bạn đã ở trong hàng chờ của khung giờ này rồi.");
            }
            var position = await _unitOfWork.BookingWaitlistRepository.GetNextPositionAsync(request.SalonId, request.RequestedDate, request.RequestedStartTime, request.PreferredNailArtistId);

            var wailist = _mapper.Map<BookingWaitlist>(request);
            wailist.CustomerId = customerId;
            wailist.Position = position;
            wailist.Status = WaitlistStatus.Waiting;
            wailist.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.BookingWaitlistRepository.CreateAsync(wailist);
            await _unitOfWork.SaveChangesAsync();
            var detailedWaitlist = await _unitOfWork.BookingWaitlistRepository.GetWaitlistWithDetailsAsync(wailist.WailistId);
            var response = _mapper.Map<WaitlistResponseDTO>(detailedWaitlist ?? wailist);
            return new ApiSuccessResult<WaitlistResponseDTO>(response, "Đăng ký vào hàng chờ thành công.");
        }
    }
}
