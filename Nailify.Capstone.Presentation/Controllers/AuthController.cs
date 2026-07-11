using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Nailify.Capstone.Application.DTOs.RequestDTOs.AuthRequestDTOs;
using Nailify.Capstone.Application.DTOs.RequestDTOs.UserRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;

namespace Nailify.Capstone.Presentation.Controllers
{
    /// <summary>
    /// API phân quyền.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResult<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Đăng nhập.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);

            if (result == null)
            {
                return Unauthorized(new
                {
                    isSucceeded = false,
                    message = "Tên đăng nhập hoặc mật khẩu không chính xác!"
                });
            }

            return Ok(new
            {
                isSucceeded = true,
                message = "Đăng nhập thành công.",
                data = new
                {
                    token = result.Token,
                }
            });
        }

        /// <summary>
        /// Mã hóa mật khẩu (BCrypt Hash) dùng để seed database hoặc tạo mật khẩu mẫu.
        /// </summary>
        [HttpGet("hash-password")]
        public IActionResult HashPassword([FromQuery] string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return BadRequest(new { isSucceeded = false, message = "Mật khẩu không được để trống." });
            }
            var hashed = BCrypt.Net.BCrypt.HashPassword(password);
            return Ok(new
            {
                isSucceeded = true,
                plainText = password,
                hashed = hashed
            });
        }
    }
}