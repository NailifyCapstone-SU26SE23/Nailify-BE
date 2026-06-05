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
                isSucceeded = false,
                status =200,
                message = "Cập nhật trang cá nhân thành công tốt đẹp!" });
        }
    }
}
