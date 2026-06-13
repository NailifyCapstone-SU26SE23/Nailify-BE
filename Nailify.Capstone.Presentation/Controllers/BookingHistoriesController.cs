using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;

namespace Nailify.Capstone.Presentation.Controllers
{
    /// <summary>
    /// API quản lý lịch sử đặt lịch.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BookingHistoriesController : ControllerBase
    {
        private readonly IBookingHistoryService _bookingHistoryService;

        public BookingHistoriesController(IBookingHistoryService bookingHistoryService)
        {
            _bookingHistoryService = bookingHistoryService;
        }

        /// <summary>
        /// Lấy danh sách lịch sử đặt lịch.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<PagedList<BookingHistoryResponseDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var result = await _bookingHistoryService.GetPagedBookingHistoriesAsync(pageNumber, pageSize, startDate, endDate);
            return Ok(result);
        }

        /// <summary>
        /// Lấy chi tiết lịch sử đặt lịch theo ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResult<BookingHistoryResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _bookingHistoryService.GetBookingHistoryByIdAsync(id);
            if (!result.IsSucceeded) return NotFound(result);
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách lịch sử đặt lịch theo booking ID
        /// </summary>
        [HttpGet("booking/{bookingId}")]
        [ProducesResponseType(typeof(ApiResult<PagedList<BookingHistoryResponseDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByBookingId(
            Guid bookingId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var result = await _bookingHistoryService.GetPagedBookingHistoriesByBookingIdAsync(bookingId, pageNumber, pageSize, startDate, endDate);
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách lịch sử đặt lịch theo salon ID
        /// </summary>
        [HttpGet("salon/{salonId}")]
        [ProducesResponseType(typeof(ApiResult<PagedList<BookingHistoryResponseDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBySalonId(
            Guid salonId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var result = await _bookingHistoryService.GetPagedBookingHistoriesBySalonIdAsync(salonId, pageNumber, pageSize, startDate, endDate);
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách lịch sử đặt lịch theo thợ làm móng ID
        /// </summary>
        [HttpGet("artist/{artistId}")]
        [ProducesResponseType(typeof(ApiResult<PagedList<BookingHistoryResponseDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByArtistId(
            Guid artistId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var result = await _bookingHistoryService.GetPagedBookingHistoriesByArtistIdAsync(artistId, pageNumber, pageSize, startDate, endDate);
            return Ok(result);
        }
    }
}