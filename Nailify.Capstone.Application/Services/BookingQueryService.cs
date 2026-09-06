using AutoMapper;
using Nailify.Capstone.Application.Common;
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
    public class BookingQueryService : IBookingQueryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookingQueryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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

        public async Task<ApiResult<PagedList<BookingResponseDTO>>> GetBookingsBySalonAsync(Guid salonId, int pageNumber, int pageSize, DateTime? startDate = null, DateTime? endDate = null, BookingStatus? status = null, string? search = null)
        {
            var bookings = await _unitOfWork.BookingRepository.GetBookingsBySalonAsync(salonId, pageNumber, pageSize, startDate, endDate, status, search);
            var response = MapPagedBookings(bookings, pageNumber, pageSize);
            return new ApiSuccessResult<PagedList<BookingResponseDTO>>(response, "Lấy danh sách đặt lịch của Salon thành công.");
        }
        private PagedList<BookingResponseDTO> MapPagedBookings(PagedList<Booking> pagedBookings, int pageNumber, int pageSize)
        {
            var mappedItems = _mapper.Map<List<BookingResponseDTO>>(pagedBookings.Items);
            return new PagedList<BookingResponseDTO>(mappedItems, pagedBookings.MetaData.TotalItems, pageNumber, pageSize);
        }
        public async Task<ApiResult<PagedList<BookingResponseDTO>>> GetBookingsByArtistAsync(Guid artistId, int pageNumber, int pageSize, DateTime? startDate = null, DateTime? endDate = null, BookingStatus? status = null, string? search = null)
        {
            var bookings = await _unitOfWork.BookingRepository.GetBookingsByArtistAsync(artistId, pageNumber, pageSize, startDate, endDate, status, search);
            var response = MapPagedBookings(bookings, pageNumber, pageSize);
            return new ApiSuccessResult<PagedList<BookingResponseDTO>>(response, "Lấy danh sách đặt lịch của Thợ làm móng thành công.");
        }
        public async Task<ApiResult<BookingIdResponseDTO>> GetBookingIdByOrderCodeAsync(long orderCode)
        {
            var bookingId = await _unitOfWork.TransactionRepository.GetBookingIdByOrderCodeAsync(orderCode.ToString());
            return bookingId.HasValue
                ? new ApiSuccessResult<BookingIdResponseDTO>(new BookingIdResponseDTO { BookingId = bookingId.Value }, "Lấy mã lịch hẹn thành công.")
                : new ApiErrorResult<BookingIdResponseDTO>("Không tìm thấy giao dịch.");
        }
        public async Task<ApiResult<PagedList<BookingResponseDTO>>> GetMyBookingsAsync(Guid customerId, int pageNumber, int pageSize, DateTime? startDate = null, DateTime? endDate = null, BookingStatus? status = null)
        {
            var bookings = await _unitOfWork.BookingRepository.GetBookingsByCustomerAsync(customerId, pageNumber, pageSize, startDate, endDate, status);
            var response = MapPagedBookings(bookings, pageNumber, pageSize);
            return new ApiSuccessResult<PagedList<BookingResponseDTO>>(response, "Lấy danh sách đặt lịch của khách hàng thành công.");
        }
        public async Task<ApiResult<BookingResponseDTO>> GetBookingDetailWithWarrantyAsync(Guid bookingId)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(bookingId);
            if (booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Không tìm thấy thông tin đặt lịch.");
            }
            var response = _mapper.Map<BookingResponseDTO>(booking);

            // Tìm đơn bảo hành của đơn này (nếu có)
            var warrantyBooking = await _unitOfWork.BookingRepository.GetWarrantyBookingAsync(bookingId);
            response.IsWarrantied = warrantyBooking != null;
            response.WarrantyBookingId = warrantyBooking?.BookingId;

            return new ApiSuccessResult<BookingResponseDTO>(response, "Lấy thông tin chi tiết đặt lịch thành công.");
        }

        public async Task<ApiResult<List<BookingResponseDTO>>> GetLateCancelledBookingsBySalonAsync(Guid salonId)
        {
            var today = DateTime.UtcNow.AddHours(7).Date;
            var bookings = await _unitOfWork.BookingRepository.GetLateCancelledBookingsBySalonAsync(salonId, today);
            var response = _mapper.Map<List<BookingResponseDTO>>(bookings);
            return new ApiSuccessResult<List<BookingResponseDTO>>(response, "Lấy danh sách các đơn bị hủy do trễ của Salon thành công.");
        }
    }
}
