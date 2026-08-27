using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface IBookingDiscountService
    {
        Task<ApiResult<BookingDiscountDto>> GetByIdAsync(int bookingDiscountId);
        Task<ApiResult<List<BookingDiscountDto>>> GetByBookingIdAsync(Guid bookingId);
    }
}
