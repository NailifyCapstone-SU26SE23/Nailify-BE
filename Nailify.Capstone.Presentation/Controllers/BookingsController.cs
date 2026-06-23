using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Enums;
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
        private readonly ISlotHoldService _slotHoldService;
        public BookingsController(IBookingService bookingService, CloudinaryService _cloudinary, ISlotHoldService slotHoldService)
        {
            _bookingService = bookingService;
            _cloudinaryService = _cloudinary;
            _slotHoldService = slotHoldService;
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
        /// Lấy thông tin một thợ ngẫu nhiên tối ưu nhất (Nhân đạo ThanhDT).
        /// </summary>
        [HttpPost("random-artist")]
        [ProducesResponseType(typeof(ApiResult<SuggestedArtistResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetRandomArtist([FromBody] GetRandomArtistRequestDTO request)
        {
            var response = await _bookingService.GetRandomArtistAsync(request);
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

        [HttpPost("price")]
        [ProducesResponseType(typeof(ApiResult<BookingPriceResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CalculatePrice([FromBody] BookingPriceRequestDTO request)
        {
            var response = await _bookingService.CalculateBookingPriceAsync(
                GetCurrentUserId(),
                request.BookingItems);
            return response.IsSucceeded ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Thực hiện chụp hình bàn tay khách cho đơn đặt lịch (Chụp hình trước khi làm).
        /// </summary>
        [HttpPost("check-in-images")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResult<BookingResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CheckIn([FromForm] CheckInForm request)
        {
            string checkInImageUrl = string.Empty;
            try
            {
                var currentUserId = GetCurrentUserId();
                if (request.Image != null && request.Image.Length > 0)
                {
                    checkInImageUrl = await _cloudinaryService.UploadImageAsync(request.Image);
                }

                var appRequest = new CheckInRequestDTO
                {
                    BookingId = request.BookingId,
                    CheckInImageUrl = checkInImageUrl
                };

                var response = await _bookingService.CheckInBookingAsync(appRequest, currentUserId);
                if (!response.IsSucceeded && !string.IsNullOrEmpty(checkInImageUrl))
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
                return BadRequest(new ApiResult<object>(false, $"Check-in thất bại: {ex.Message}"));
            }
        }

        /// <summary>
        /// Thực hiện Check-out cho đơn đặt lịch (Sau khi thợ hoàn thành dịch vụ xong và khách thanh toán thành công).
        /// </summary>
        [HttpPost("check-out")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResult<BookingResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CheckOut([FromForm] CheckOutRequestDTO request)
        {
            var currentUserId = GetCurrentUserId();
            var response = await _bookingService.CheckOutBookingAsync(request, currentUserId);
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
            var customerId = GetCurrentUserId();
            var response = await _bookingService.UpdateBookingAsync(id, request, customerId);
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
            var currentUserId = GetCurrentUserId();
            var response = await _bookingService.ConfirmBookingAsync(id, currentUserId);
            if (!response.IsSucceeded) return BadRequest(response);
            return Ok(response);
        }

        /// <summary>
        /// Tiếp tân checkin lịch hẹn bằng tay.
        /// </summary>
        [HttpPost("{id}/manual-checkin")]
        [ProducesResponseType(typeof(ApiResult<BookingResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ManualCheckIn(Guid id)
        {
            var currentUserId = GetCurrentUserId();
            var response = await _bookingService.ManualCheckInBookingAsync(id, currentUserId);
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
        public async Task<IActionResult> Reject(Guid id, [FromBody] RejectRequestDTO request)
        {
            var currentUserId = GetCurrentUserId();
            var response = await _bookingService.RejectBookingAsync(id, currentUserId, request);
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
            var currentUserId = GetCurrentUserId();
            var response = await _bookingService.StartServiceAsync(id, currentUserId);
            if (!response.IsSucceeded) return BadRequest(response);
            return Ok(response);
        }
        /// <summary>
        /// Khách hàng xem lịch sử đặt hẹn của chính mình.
        /// </summary>
        [HttpGet("my-bookings")]
        [ProducesResponseType(typeof(ApiResult<PagedList<BookingResponseDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMyBookings(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] BookingStatus? status = null)
        {
            var customerId = GetCurrentUserId();
            var response = await _bookingService.GetMyBookingsAsync(customerId, pageNumber, pageSize, startDate, endDate, status);
            if (!response.IsSucceeded) return BadRequest(response);
            return Ok(response);
        }
        /// <summary>
        /// Quản lý Salon xem toàn bộ danh sách lịch hẹn của Salon đó (có thể lọc theo ngày).
        /// </summary>
        [HttpGet("salon/{salonId}")]
        [ProducesResponseType(typeof(ApiResult<PagedList<BookingResponseDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetSalonBookings(
            Guid salonId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] BookingStatus? status = null,
            [FromQuery] string? search = null)
        {
            var response = await _bookingService.GetBookingsBySalonAsync(salonId, pageNumber, pageSize, startDate, endDate, status, search);
            if (!response.IsSucceeded) return BadRequest(response);
            return Ok(response);
        }
        /// <summary>
        /// Thợ làm móng xem danh sách lịch hẹn được giao của chính mình.
        /// </summary>
        [HttpGet("artist/{artistId}")]
        [ProducesResponseType(typeof(ApiResult<PagedList<BookingResponseDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetArtistBookings(
            Guid artistId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] BookingStatus? status = null,
            [FromQuery] string? search = null)
        {
            var response = await _bookingService.GetBookingsByArtistAsync(artistId, pageNumber, pageSize, startDate, endDate, status, search);
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
            var currentUserId = GetCurrentUserId();
            var response = await _bookingService.VerifyQrCodeAsync(qrToken, currentUserId);
            if (!response.IsSucceeded) return BadRequest(response);
            return Ok(response);
        }

        /// <summary>
        /// Thợ nail (hoặc Lễ tân) báo hoàn thành các bước làm móng và tải lên ảnh móng hoàn chỉnh.
        /// </summary>
        [HttpPost("{id}/complete-service")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResult<BookingResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CompleteService(Guid id, [FromForm] CompleteServiceForm request)
        {
            if (request.Images == null || !request.Images.Any())
            {
                return BadRequest(new ApiResult<object>(false, "Vui lòng chụp/tải lên ảnh móng tay sau khi làm xong."));
            }
            var uploadedUrls = new List<string>();
            try
            {
                var currentUserId = GetCurrentUserId();
                uploadedUrls = await _cloudinaryService.UploadMultipleImagesAsync(request.Images);
                var appRequest = new CompleteServiceRequestDTO
                {
                    BookingId = id,
                    CompleteImagesUrl = uploadedUrls
                };
                var response = await _bookingService.CompleteServiceAsync(appRequest, currentUserId);
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
                return BadRequest(new ApiResult<object>(false, $"Hoàn thành dịch vụ thất bại khi tải ảnh: {ex.Message}"));
            }
        }
        /// <summary>
        /// Giữ chỗ slot 5 phút để khách hàng có thời gian chọn dịch vụ.
        /// </summary>
        [HttpPost("hold-slot")]
        [ProducesResponseType(typeof(ApiResult<SlotHoldResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> HoldSlot([FromBody] HoldSlotRequestDTO request)
        {
            try
            {
                var customerId = GetCurrentUserId();
                var response = await _slotHoldService.HoldSlotAsync(customerId, request);
                if (!response.IsSucceeded) return BadRequest(response);
                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                return UnauthorizedResponse();
            }
        }
        /// <summary>
        /// Hủy giữ chỗ slot thủ công (khi khách đổi ý).
        /// </summary>
        [HttpDelete("hold-slot/{holdToken}")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ReleaseHold(string holdToken)
        {
            try
            {
                var customerId = GetCurrentUserId();
                var response = await _slotHoldService.ReleaseSlotAsync(customerId, holdToken);
                if (!response.IsSucceeded) return BadRequest(response);
                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                return UnauthorizedResponse();
            }
        }
        /// <summary>
        /// Kiểm tra trạng thái giữ chỗ (còn hiệu lực không, còn bao nhiêu giây).
        /// </summary>
        [HttpGet("hold-slot/{holdToken}/status")]
        [ProducesResponseType(typeof(ApiResult<SlotHoldResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetHoldStatus(string holdToken)
        {
            var response = await _slotHoldService.GetHoldStatusAsync(holdToken);
            if (!response.IsSucceeded) return BadRequest(response);
            return Ok(response);
        }
        /// <summary>
        /// Tiếp tân (hoặc Quản lý) chỉ định thợ nail cho đơn đặt lịch khi khách đến.
        /// </summary>
        [HttpPost("{id}/receptionist-assign-artist")]
        [ProducesResponseType(typeof(ApiResult<BookingResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ReceptionistAssignArtist(Guid id, [FromBody] AssignArtistRequestDTO request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var response = await _bookingService.ReceptionistAssignArtistAsync(id, request, currentUserId);
                if (!response.IsSucceeded) return BadRequest(response);
                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                return UnauthorizedResponse();
            }
        }
        /// <summary>
        /// Lấy danh sách thợ làm móng đang rảnh và đủ điều kiện làm cho đơn đặt lịch (dùng cho lễ tân phân thợ khi khách đến).
        /// </summary>
        [HttpGet("{id}/available-artists-for-receptionist")]
        [ProducesResponseType(typeof(ApiResult<List<SuggestedArtistResponseDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAvailableArtistsForBooking(Guid id)
        {
            var response = await _bookingService.GetAvailableArtistsForBookingAsync(id);
            if (!response.IsSucceeded) return BadRequest(response);
            return Ok(response);
        }
    }

    public class CheckInForm
    {
        public Guid BookingId { get; set; }
        public IFormFile? Image { get; set; }
    }
    public class CompleteServiceForm
    {
        public List<IFormFile> Images { get; set; } = new();
    }
}
