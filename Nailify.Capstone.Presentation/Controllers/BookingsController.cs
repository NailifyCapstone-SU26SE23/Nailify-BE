using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Infrastructure.Service;
using Nailify.Capstone.Presentation.Middlewares;
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
        private readonly CloudinaryService _cloudinaryService;

        public BookingsController(IBookingService bookingService, CloudinaryService _cloudinary)
        {
            _bookingService = bookingService;
            _cloudinaryService = _cloudinary;
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
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResult<BookingResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CheckIn([FromForm] CheckInForm request)
        {
            if (request.Image == null || request.Image.Length == 0)
            {
                return BadRequest(new ApiResult<object>(false, "Vui lòng chụp/tải lên ảnh check-in."));
            }

            string checkInImageUrl = string.Empty;
            try
            {
                checkInImageUrl = await _cloudinaryService.UploadImageAsync(request.Image);

                var appRequest = new CheckInRequestDTO
                {
                    BookingId = request.BookingId,
                    CheckInImageUrl = checkInImageUrl
                };

                var response = await _bookingService.CheckInBookingAsync(appRequest);
                if (!response.IsSucceeded)
                {
                    await _cloudinaryService.DeleteImageAsync(checkInImageUrl);
                    return BadRequest(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(checkInImageUrl))
                {
                    await _cloudinaryService.DeleteImageAsync(checkInImageUrl);
                }
                return BadRequest(new ApiResult<object>(false, $"Check-in thất bại khi tải ảnh: {ex.Message}"));
            }
        }

        /// <summary>
        /// Thực hiện Check-out cho đơn đặt lịch (Chụp hình hoàn thành dịch vụ).
        /// </summary>
        [HttpPost("check-out")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResult<BookingResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CheckOut([FromForm] CheckOutForm request)
        {
            if (request.Images == null || !request.Images.Any())
            {
                return BadRequest(new ApiResult<object>(false, "Vui lòng chụp/tải lên ảnh check-out."));
            }

            var uploadedUrls = new List<string>();
            try
            {
                uploadedUrls = await _cloudinaryService.UploadMultipleImagesAsync(request.Images);

                var appRequest = new CheckOutRequestDTO
                {
                    BookingId = request.BookingId,
                    CheckOutImagesUrl = uploadedUrls
                };

                var response = await _bookingService.CheckOutBookingAsync(appRequest);
                if (!response.IsSucceeded)
                {
                    await _cloudinaryService.DeleteMultipleImagesAsync(uploadedUrls);
                    return BadRequest(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                if (uploadedUrls.Any())
                {
                    await _cloudinaryService.DeleteMultipleImagesAsync(uploadedUrls);
                }
                return BadRequest(new ApiResult<object>(false, $"Check-out thất bại khi tải ảnh: {ex.Message}"));
            }
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

        /// <summary>
        /// Khách hàng hoặc Quản lý Salon hủy đơn đặt lịch hẹn.
        /// </summary>
        [HttpPost("{id}/cancel")]
        [ProducesResponseType(typeof(ApiResult<BookingResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelBookingRequestDTO request)
        {
            var customerId = GetCurrentUserId();
            var response = await _bookingService.CancelBookingAsync(id, customerId, request);
            if (!response.IsSucceeded) return BadRequest(response);
            return Ok(response);
        }

        /// <summary>
        /// Quản lý Salon xác nhận duyệt đơn đặt lịch hẹn.
        /// </summary>
        [HttpPost("{id}/confirm")]
        [ProducesResponseType(typeof(ApiResult<BookingResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Confirm(Guid id)
        {
            var response = await _bookingService.ConfirmBookingAsync(id);
            if (!response.IsSucceeded) return BadRequest(response);
            return Ok(response);
        }

        /// <summary>
        /// Quản lý Salon từ chối đơn đặt lịch hẹn.
        /// </summary>
        [HttpPost("{id}/reject")]
        [ProducesResponseType(typeof(ApiResult<BookingResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Reject(Guid id)
        {
            var response = await _bookingService.RejectBookingAsync(id);
            if (!response.IsSucceeded) return BadRequest(response);
            return Ok(response);
        }

        /// <summary>
        /// Thợ làm móng (Nail Artist) xác nhận bắt đầu thực hiện làm móng cho khách.
        /// </summary>
        [HttpPost("{id}/start")]
        [ProducesResponseType(typeof(ApiResult<BookingResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Start(Guid id)
        {
            var response = await _bookingService.StartServiceAsync(id);
            if (!response.IsSucceeded) return BadRequest(response);
            return Ok(response);
        }
        /// <summary>
        /// Khách hàng xem lịch sử đặt hẹn của chính mình.
        /// </summary>
        [HttpGet("my-bookings")]
        [ProducesResponseType(typeof(ApiResult<IEnumerable<BookingResponseDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMyBookings()
        {
            var customerId = GetCurrentUserId();
            var response = await _bookingService.GetMyBookingsAsync(customerId);
            if (!response.IsSucceeded) return BadRequest(response);
            return Ok(response);
        }
        /// <summary>
        /// Quản lý Salon xem toàn bộ danh sách lịch hẹn của Salon đó (có thể lọc theo ngày).
        /// </summary>
        [HttpGet("salon/{salonId}")]
        [ProducesResponseType(typeof(ApiResult<IEnumerable<BookingResponseDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetSalonBookings(Guid salonId, [FromQuery] DateTime? date)
        {
            var response = await _bookingService.GetBookingsBySalonAsync(salonId, date);
            if (!response.IsSucceeded) return BadRequest(response);
            return Ok(response);
        }
        /// <summary>
        /// Thợ làm móng xem danh sách lịch hẹn được giao của chính mình.
        /// </summary>
        [HttpGet("artist/{artistId}")]
        [ProducesResponseType(typeof(ApiResult<IEnumerable<BookingResponseDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetArtistBookings(Guid artistId)
        {
            var response = await _bookingService.GetBookingsByArtistAsync(artistId);
            if (!response.IsSucceeded) return BadRequest(response);
            return Ok(response);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một đơn đặt lịch hẹn cụ thể.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResult<BookingResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _bookingService.GetBookingByIdAsync(id);
            if (!response.IsSucceeded) return BadRequest(response);
            return Ok(response);
        }

        // 3. API Xác thực QR
        /// <summary>
        /// Xác thực chuỗi mã QR do lễ tân quét để kiểm tra tính hợp lệ trước khi Check-in.
        /// </summary>
        [HttpPost("verify-qr")]
        [ProducesResponseType(typeof(ApiResult<BookingResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> VerifyQr([FromQuery] string qrToken)
        {
            var response = await _bookingService.VerifyQrCodeAsync(qrToken);
            if (!response.IsSucceeded) return BadRequest(response);
            return Ok(response);
        }
    }

    public class CheckInForm
    {
        public Guid BookingId { get; set; }
        public IFormFile Image { get; set; } = null!;
    }

    public class CheckOutForm
    {
        public Guid BookingId { get; set; }
        public List<IFormFile> Images { get; set; } = new();
    }
}
