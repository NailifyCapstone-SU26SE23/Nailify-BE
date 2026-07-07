using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.WaitlistRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.WaitlistResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using System;
using System.Threading.Tasks;

namespace Nailify.Capstone.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WaitlistsController : BaseApiController
    {
        private readonly IBookingWaitlistService _waitlistService;

        public WaitlistsController(IBookingWaitlistService waitlistService)
        {
            _waitlistService = waitlistService;
        }

        /// <summary>
        /// Đăng ký tham gia hàng chờ (Waitlist) khi slot mong muốn bị đầy.
        /// </summary>
        /// <param name="request">Thông tin yêu cầu hàng chờ gồm SalonId, ngày giờ và thợ yêu thích.</param>
        /// <returns>Thông tin vị trí hàng chờ được khởi tạo thành công.</returns>
        [HttpPost("join")]
        [ProducesResponseType(typeof(ApiResult<WaitlistResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Join([FromBody] JoinWaitlistRequestDTO request)
        {
            var customerId = GetCurrentUserId();
            var result = await _waitlistService.JoinWaitlistAsync(customerId, request);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Khách hàng xác nhận nhận chỗ khi slot trống được mở và hệ thống gửi thông báo.
        /// </summary>
        /// <param name="id">Mã định danh (ID) của hàng chờ cần xác nhận.</param>
        /// <returns>Thông tin hàng chờ được xác nhận và thông tin Booking được tạo kèm.</returns>
        [HttpPost("{id}/confirm")]
        [ProducesResponseType(typeof(ApiResult<WaitlistResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Confirm(Guid id, [FromBody(EmptyBodyBehavior = Microsoft.AspNetCore.Mvc.ModelBinding.EmptyBodyBehavior.Allow)] ConfirmWaitlistRequestDTO? request = null)
        {
            var customerId = GetCurrentUserId();
            request ??= new ConfirmWaitlistRequestDTO();
            var result = await _waitlistService.ConfirmWaitlistAsync(id, customerId, request);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Hủy tham gia hàng chờ (Rút khỏi hàng chờ).
        /// </summary>
        /// <param name="id">Mã định danh (ID) của hàng chờ cần hủy.</param>
        /// <returns>Kết quả hủy vị trí hàng chờ.</returns>
        [HttpPost("{id}/cancel")]
        [ProducesResponseType(typeof(ApiResult<WaitlistResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var customerId = GetCurrentUserId();
            var result = await _waitlistService.CancelWaitlistAsync(id, customerId);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Lấy tất cả lượt đăng ký hàng chờ đang hoạt động của khách hàng hiện tại.
        /// </summary>
        /// <returns>Danh sách hàng chờ đang hoạt động của khách hàng.</returns>
        [HttpGet("me")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(ApiResult<List<WaitlistResponseDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetMyWaitlists()
        {
            var customerId = GetCurrentUserId();
            var result = await _waitlistService.GetMyWaitlistsAsync(customerId);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Lấy danh sách hàng chờ phân trang theo chi nhánh Salon.
        /// </summary>
        /// <param name="salonId">Mã định danh (ID) của chi nhánh Salon.</param>
        /// <param name="pageNumber">Số trang hiện tại (mặc định là 1).</param>
        /// <param name="pageSize">Kích thước trang (mặc định là 10).</param>
        /// <returns>Danh sách hàng chờ phân trang.</returns>
        [HttpGet("salon/{salonId}")]
        [ProducesResponseType(typeof(ApiResult<PagedList<WaitlistResponseDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSalonWaitlist(Guid salonId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _waitlistService.GetSalonWaitlistAsync(salonId, pageNumber, pageSize);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }
    }
}
