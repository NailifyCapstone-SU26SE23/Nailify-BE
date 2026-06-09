using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nailify.Capstone.Presentation.Controllers
{
    /// <summary>
    /// API quản lý đặt lịch (Booking).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : BaseApiController
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        /// <summary>
        /// Lấy danh sách thợ làm móng đề xuất dựa trên các mẫu móng đã chọn.
        /// </summary>
        [HttpPost("suggested-artists")]
        [ProducesResponseType(typeof(ApiResult<List<SuggestedArtistResponseDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSuggestedArtists([FromBody] GetSuggestedArtistsRequestDTO request)
        {
            var response = await _bookingService.GetSuggestedArtistAsync(request);
            if (!response.IsSucceeded) return BadRequest(response);
            return Ok(response);
        }

        /// <summary>
        /// Lấy danh sách các khung giờ bận của thợ làm móng trong ngày cụ thể.
        /// </summary>
        [HttpGet("artist-available-slots")]
        [ProducesResponseType(typeof(ApiResult<ArtistAvailabilityResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetArtistAvailableSlots([FromQuery] GetArtistAvailableSlotsRequestDTO request)
        {
            var response = await _bookingService.GetArtistAvailableSlotAsync(request);
            if (!response.IsSucceeded) return BadRequest(response);
            return Ok(response);
        }

        /// <summary>
        /// Tạo mới một đơn đặt lịch (Booking).
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<BookingResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Create([FromBody] CreateBookingRequestDTO request)
        {
            try
            {
                var customerId = GetCurrentUserId();
                var response = await _bookingService.CreateBookingAsync(customerId, request);
                if (!response.IsSucceeded) return BadRequest(response);
                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                return UnauthorizedResponse();
            }
        }

        /// <summary>
        /// Thực hiện Check-in cho đơn đặt lịch (Chụp hình trước khi làm).
        /// </summary>
        [HttpPost("check-in")]
        [ProducesResponseType(typeof(ApiResult<BookingResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CheckIn([FromBody] CheckInRequestDTO request)
        {
            var response = await _bookingService.CheckInBookingAsync(request);
            if (!response.IsSucceeded) return BadRequest(response);
            return Ok(response);
        }

        /// <summary>
        /// Thực hiện Check-out cho đơn đặt lịch (Chụp hình hoàn thành dịch vụ).
        /// </summary>
        [HttpPost("check-out")]
        [ProducesResponseType(typeof(ApiResult<BookingResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CheckOut([FromBody] CheckOutRequestDTO request)
        {
            var response = await _bookingService.CheckOutBookingAsync(request);
            if (!response.IsSucceeded) return BadRequest(response);
            return Ok(response);
        }

        /// <summary>
        /// Cập nhật thông tin đơn đặt lịch (Thay đổi ngày giờ, thợ, hoặc các mẫu móng/dịch vụ đặt kèm).
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResult<BookingResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBookingRequestDTO request)
        {
            var response = await _bookingService.UpdateBookingAsync(id, request);
            if (!response.IsSucceeded) return BadRequest(response);
            return Ok(response);
        }

        /// <summary>
        /// Tạo mới một đơn đặt lịch tùy chỉnh (Booking Custom) của khách hàng.
        /// </summary>
        [HttpPost("custom")]
        [ProducesResponseType(typeof(ApiResult<BookingResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateCustom([FromBody] CreateCustomBookingRequestDTO request)
        {
            try
            {
                var customerId = GetCurrentUserId();
                var response = await _bookingService.CreateCustomBookingAsync(customerId, request);
                if (!response.IsSucceeded) return BadRequest(response);
                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                return UnauthorizedResponse();
            }
        }

        /// <summary>
        /// Salon Manager phân bổ thợ nail cho đơn đặt lịch tùy chỉnh.
        /// </summary>
        [HttpPost("{id}/assign-artist")]
        [ProducesResponseType(typeof(ApiResult<BookingResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AssignArtist(Guid id, [FromBody] AssignArtistRequestDTO request)
        {
            var response = await _bookingService.AssignArtistAsync(id, request);
            if (!response.IsSucceeded) return BadRequest(response);
            return Ok(response);
        }

        /// <summary>
        /// Thợ nail đề xuất giá và thời gian cho đơn đặt lịch tùy chỉnh.
        /// </summary>
        [HttpPost("{id}/artist-quote")]
        [ProducesResponseType(typeof(ApiResult<BookingResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ArtistQuote(Guid id, [FromBody] ArtistQuoteRequestDTO request)
        {
            var response = await _bookingService.ArtistQuoteAsync(id, request);
            if (!response.IsSucceeded) return BadRequest(response);
            return Ok(response);
        }

        /// <summary>
        /// Salon Manager chốt/duyệt báo giá cuối cùng cho đơn đặt lịch tùy chỉnh.
        /// </summary>
        [HttpPost("{id}/manager-approve-quote")]
        [ProducesResponseType(typeof(ApiResult<BookingResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ManagerApproveQuote(Guid id, [FromBody] ManagerApproveQuoteRequestDTO request)
        {
            var response = await _bookingService.ManagerApproveQuoteAsync(id, request);
            if (!response.IsSucceeded) return BadRequest(response);
            return Ok(response);
        }
    }
}
