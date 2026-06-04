using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Nailify.Capstone.Application.DTOs.RequestDTOs.UserRequestDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Presentation.Middlewares;
namespace Nailify.Capstone.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly IUserService _userService;

        public ProfileController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// API Xem thông tin cá nhân của người dùng đang đăng nhập
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetMyProfile()
        {
            // Trích xuất UserId từ Token JWT đã giải mã qua Middleware
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new {
                    isSucceeded = false,
                    status = 401,
                    message = " Token không hợp lệ" });
            }

            var userId = Guid.Parse(userIdClaim);
            var profile = await _userService.GetProfileAsync(userId);

            if (profile == null)
            {
                return NotFound(new {
                    isSucceeded = false,
                    status = 404,
                    message = "Tài khoản không tồn tại hoặc đã bị khóa." });
            }

            return Ok(profile);
        }

        /// <summary>
        /// API Cập nhật thông tin cá nhân của người dùng đang đăng nhập
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> UpdateMyProfile([FromBody] ProfileUpdateRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new {
                    isSucceeded = false,
                    status = 401,
                    message = "Token không hợp lệ!" });
            }

            var userId = Guid.Parse(userIdClaim);
            var isSuccess = await _userService.UpdateProfileAsync(userId, request);

            if (!isSuccess)
            {
                return BadRequest(new {
                    isSucceeded = false,
                    status = 400,
                    message = "Cập nhật thông tin cá nhân thất bại." });
            }

            return Ok(new {
                isSucceeded = true,
                status =200,
                message = "Cập nhật trang cá nhân thành công tốt đẹp!" });
        }

        /// <summary>
        /// Khách hàng tự truy xuất thông tin hồ sơ cá nhân tổng hợp (Gồm cả bảng User và Customer)
        /// </summary>
        [HttpGet("customers")]
        //[HasRole("Customer")]
        public async Task<IActionResult> GetMyCustomerProfile()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new { message = "Mã token không hợp lệ hoặc thiếu thông tin định danh cá nhân." });
            }

            var result = await _userService.GetCustomerProfileAsync(Guid.Parse(userIdClaim));
            return Ok(result);
        }

        /// <summary>
        /// Khách hàng tự cập nhật đặc điểm da và lối sống cá nhân phục vụ gợi ý móng mẫu thích hợp
        /// </summary>
        [HttpPut("customers/preferences")]
        //[HasRole("Customer")]
        public async Task<IActionResult> UpdateMyPreferences([FromBody] CustomerPreferencesUpdateRequest request)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new { message = "Mã token không hợp lệ hoặc thiếu thông tin định danh cá nhân." });
            }

            var result = await _userService.UpdateCustomerPreferencesAsync(Guid.Parse(userIdClaim), request);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
