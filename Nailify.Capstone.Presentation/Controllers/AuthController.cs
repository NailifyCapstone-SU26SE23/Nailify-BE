using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Google.Apis.Auth;
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

        [HttpPost("google")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            try
            {
                var result = await _authService.GoogleLoginAsync(request);
                if (result == null)
                {
                    return Unauthorized(new
                    {
                        isSucceeded = false,
                        message = "Google token khÃ´ng há»£p lá»‡."
                    });
                }

                return Ok(new
                {
                    isSucceeded = true,
                    message = "ÄÄƒng nháº­p Google thÃ nh cÃ´ng.",
                    data = new
                    {
                        token = result.Token,
                    }
                });
            }
            catch (InvalidJwtException)
            {
                return Unauthorized(new
                {
                    isSucceeded = false,
                    message = "Google token khÃ´ng há»£p lá»‡."
                });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    isSucceeded = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost("forgot-password")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var result = await _authService.ForgotPasswordAsync(request);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("check-reset-token")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CheckResetPasswordToken([FromBody] CheckResetPasswordTokenRequest request)
        {
            var result = await _authService.CheckResetPasswordTokenAsync(request);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("reset-password")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var result = await _authService.ResetPasswordAsync(request);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
