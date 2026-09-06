using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingResponseDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface IBookingCreationService
    {
        Task<ApiResult<BookingResponseDTO>> CreateBookingAsync(Guid customerId, CreateBookingRequestDTO request);
        Task<ApiResult<BookingPriceResponseDTO>> CalculateBookingPriceAsync(
            Guid? customerId,
            IEnumerable<BookingItemRequestDTO> bookingItems,
            List<int>? selectedPromotionIds = null);
    }
}
