using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;

namespace Nailify.Capstone.Application.Services
{
    public class BookingDiscountService : IBookingDiscountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookingDiscountService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResult<BookingDiscountDto>> GetByIdAsync(int bookingDiscountId)
        {
            var bookingDiscount = await _unitOfWork.BookingDiscountRepository.GetByIdAsync(bookingDiscountId);
            if (bookingDiscount == null)
            {
                return new ApiErrorResult<BookingDiscountDto>("Không tìm thấy giảm giá của lịch hẹn.");
            }

            return new ApiSuccessResult<BookingDiscountDto>(
                _mapper.Map<BookingDiscountDto>(bookingDiscount),
                "Lấy thông tin giảm giá của lịch hẹn thành công.");
        }

        public async Task<ApiResult<List<BookingDiscountDto>>> GetByBookingIdAsync(Guid bookingId)
        {
            if (!await _unitOfWork.BookingRepository.ExistsAsync(booking => booking.BookingId == bookingId))
            {
                return new ApiErrorResult<List<BookingDiscountDto>>("Không tìm thấy lịch hẹn.");
            }

            var bookingDiscounts = await _unitOfWork.BookingDiscountRepository.GetByBookingIdAsync(bookingId);
            return new ApiSuccessResult<List<BookingDiscountDto>>(
                _mapper.Map<List<BookingDiscountDto>>(bookingDiscounts),
                "Lấy danh sách giảm giá của lịch hẹn thành công.");
        }
    }
}
