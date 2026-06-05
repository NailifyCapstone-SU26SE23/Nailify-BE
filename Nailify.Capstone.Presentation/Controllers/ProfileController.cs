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
        [HasRole("Admin", "Customer", "Staff_Artist", "Manager")] // Cho phép tất cả các role gọi
        public async Task<IActionResult> GetMyProfile()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new { message = "Mã token không hợp lệ hoặc thiếu thông tin định danh." });
            }

            var userId = Guid.Parse(userIdClaim);

            var result = await _userService.GetProfileAsync(userId);

            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// API Cập nhật thông tin cá nhân của người dùng đang đăng nhập
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> UpdateMyProfile([FromBody] ProfileUpdateRequest request)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new { message = "Không tìm thấy thông tin định danh trong Token!" });
            }

            var userId = Guid.Parse(userIdClaim);

            // Gọi hàm và nhận về ApiResult<bool>
            var result = await _userService.UpdateProfileAsync(userId, request);

            // Nếu thất bại (IsSucceeded = false), trả về mã 4 xị và xỉn 
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            // Nếu thành công, trả về mã 200 thì ngon
            return Ok(result);
        }

        /// <summary>
        /// Khách hàng tự truy xuất thông tin hồ sơ cá nhân tổng hợp (Gồm cả bảng User và Customer)
        /// </summary>
        [HttpGet("customers")]
        [HasRole("Customer")]
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
        [HasRole("Customer")]
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
