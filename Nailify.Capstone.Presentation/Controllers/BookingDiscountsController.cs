using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;

namespace Nailify.Capstone.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingDiscountsController : ControllerBase
    {
        private readonly IBookingDiscountService _bookingDiscountService;

        public BookingDiscountsController(IBookingDiscountService bookingDiscountService)
        {
            _bookingDiscountService = bookingDiscountService;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResult<BookingDiscountDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _bookingDiscountService.GetByIdAsync(id);
            return result.IsSucceeded ? Ok(result) : NotFound(result);
        }

        [HttpGet("booking/{bookingId}")]
        [ProducesResponseType(typeof(ApiResult<List<BookingDiscountDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByBookingId(Guid bookingId)
        {
            var result = await _bookingDiscountService.GetByBookingIdAsync(bookingId);
            return result.IsSucceeded ? Ok(result) : NotFound(result);
        }
    }
}
