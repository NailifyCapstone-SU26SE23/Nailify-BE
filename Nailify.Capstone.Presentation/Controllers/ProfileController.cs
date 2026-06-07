using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.DTOs.RequestDTOs.UserRequestDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
namespace Nailify.Capstone.Presentation.Controllers
{
    /// <summary>
    /// API quản lý trang cá nhân.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : BaseApiController
    {
        private readonly IUserService _userService;

        public ProfileController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Lấy thông tin người dùng.
        /// </summary>
        [HttpGet]
        //[HasRole("Admin", "Customer", "Staff_Artist", "Manager")] // Cho phép tất cả các role gọi
        public async Task<IActionResult> GetMyProfile()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new { message = "Mã token không hợp lệ hoặc thiếu thông tin định danh." });
            }

            var userId = Guid.Parse(userIdClaim);

            var result = await _userService.GetUserByIdAsync(userId);

            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            // Kiểm tra tài khoản còn Active không
            if (result.Data?.Status == "InActive")
            {
                return Unauthorized(new { message = "Tài khoản đã bị vô hiệu hóa." });
            }

            return Ok(result);
        }

        /// <summary>
        /// API cập nhật thông tin người dùng.
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

            var result = await _userService.GetCustomerProfileByIdAsync(Guid.Parse(userIdClaim));
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }
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

        /// <summary>
        /// Khách hàng tự cập nhật đầy đủ hồ sơ và đặc điểm da/lối sống cá nhân (Persona).
        /// </summary>
        [HttpPut("customers")]
        [HasRole("Customer")]
        public async Task<IActionResult> UpdateMyCustomerProfile([FromBody] CustomerSelfProfileUpdateRequest request)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new { message = "Mã token không hợp lệ hoặc thiếu thông tin định danh cá nhân." });
            }

            var result = await _userService.UpdateCustomerSelfProfileAsync(Guid.Parse(userIdClaim), request);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Người dùng tự vô hiệu hóa tài khoản của chính mình.
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> DeleteMyAccount()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new { message = "Mã token không hợp lệ hoặc thiếu thông tin định danh cá nhân." });
            }

            var result = await _userService.DeleteUserAsync(Guid.Parse(userIdClaim));
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
