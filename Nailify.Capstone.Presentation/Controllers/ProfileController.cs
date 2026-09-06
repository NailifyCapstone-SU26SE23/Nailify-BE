using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.UserRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Infrastructure.Service;
using Nailify.Capstone.Presentation.Middlewares;
using System.IdentityModel.Tokens.Jwt;
namespace Nailify.Capstone.Presentation.Controllers
{
    /// <summary>
    /// API quản lý trang cá nhân.
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ProfileController : BaseApiController
    {
        private readonly IUserService _userService;
        private readonly CloudinaryService _cloudinaryService;

        public ProfileController(IUserService userService, CloudinaryService cloudinaryService)
        {
            _userService = userService;
            _cloudinaryService = cloudinaryService;
        }

        /// <summary>
        /// Lấy thông tin người dùng.
        /// </summary>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResult<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMyProfile()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
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
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateMyProfile([FromForm] ProfileUpdateRequest request, IFormFile? image)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new { message = "Không tìm thấy thông tin định danh trong Token!" });
            }

            var userId = Guid.Parse(userIdClaim);

            var existingResult = await _userService.GetUserByIdAsync(userId);
            if (!existingResult.IsSucceeded)
            {
                return NotFound(existingResult);
            }

            var uploadedAvatarUrl = string.Empty;
            try
            {
                uploadedAvatarUrl = await UploadImageAsync(image);
                var result = await _userService.UpdateProfileAsync(userId, request, uploadedAvatarUrl);
                if (!result.IsSucceeded)
                {
                    await DeleteImageAsync(uploadedAvatarUrl);
                    return BadRequest(result);
                }

                if (!string.IsNullOrWhiteSpace(uploadedAvatarUrl))
                {
                    await DeleteImageAsync(existingResult.Data.AvatarUrl);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                await DeleteImageAsync(uploadedAvatarUrl);
                return BadRequest(new { isSucceeded = false, message = $"Cap nhat profile that bai khi tai avatar: {ex.Message}" });
            }
        }

        /// <summary>
        /// Cập nhật mật khẩu.
        /// </summary>
        [HttpPut("password")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateMyPassword([FromBody] UpdatePasswordRequest request)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new { message = "Vui lòng đăng nhập." });
            }

            var result = await _userService.UpdatePasswordAsync(Guid.Parse(userIdClaim), request);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Khách hàng tự truy xuất thông tin hồ sơ cá nhân tổng hợp (Gồm cả bảng User và Customer)
        /// </summary>
        [HttpGet("customers")]
        public async Task<IActionResult> GetMyCustomerProfile()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
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
        public async Task<IActionResult> UpdateMyPreferences([FromBody] CustomerPreferencesUpdateRequest request)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
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
        public async Task<IActionResult> UpdateMyCustomerProfile([FromBody] CustomerSelfProfileUpdateRequest request)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
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
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
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

        private async Task<string> UploadImageAsync(IFormFile? image)
        {
            if (image == null || image.Length == 0)
            {
                return string.Empty;
            }

            return await _cloudinaryService.UploadImageAsync(image);
        }

        private async Task DeleteImageAsync(string? imageUrl)
        {
            if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                await _cloudinaryService.DeleteImageAsync(imageUrl);
            }
        }
    }
}
