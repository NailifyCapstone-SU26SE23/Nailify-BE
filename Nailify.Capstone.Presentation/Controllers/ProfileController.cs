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
        public async Task<IActionResult> GetMyProfile()
        {
            try
            {
                var userId = GetCurrentUserId();
                var profile = await _userService.GetProfileAsync(userId);

                if (profile == null)
                {
                    return NotFound(new
                    {
                        isSucceeded = false,
                        status = 404,
                        message = "Tài khoản không tồn tại hoặc đã bị khóa."
                    });
                }

                return Ok(profile);
            }
            catch (UnauthorizedAccessException)
            {
                return UnauthorizedResponse();
            }
        }

        /// <summary>
        /// API cập nhật thông tin người dùng.
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> UpdateMyProfile([FromBody] ProfileUpdateRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var isSuccess = await _userService.UpdateProfileAsync(userId, request);

                if (!isSuccess)
                {
                    return BadRequest(new
                    {
                        isSucceeded = false,
                        status = 400,
                        message = "Cập nhật thông tin cá nhân thất bại."
                    });
                }

                return Ok(new
                {
                    isSucceeded = true, 
                    status = 200,
                    message = "Cập nhật trang cá nhân thành công!"
                });
            }
            catch (UnauthorizedAccessException)
            {
                return UnauthorizedResponse();
            }
        }
    }
}
