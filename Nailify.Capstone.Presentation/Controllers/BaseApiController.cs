using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Nailify.Capstone.Presentation.Controllers
{
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        protected Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("Đã hết hạn token. Vui lòng đăng nhập lại");
            }

            return Guid.Parse(userIdClaim);
        }

        protected IActionResult UnauthorizedResponse()
        {
            return Unauthorized(new
            {
                isSucceeded = false,
                status = 401,
                message = "Đã hết hạn token. Vui lòng đăng nhập lại"
            });
        }

        protected IActionResult ErrorResponse(int statusCode, string message)
        {
            return statusCode switch
            {
                400 => BadRequest(new { isSucceeded = false, status = 400, message }),
                404 => NotFound(new { isSucceeded = false, status = 404, message }),
                _ => StatusCode(statusCode, new { isSucceeded = false, status = statusCode, message })
            };
        }
    }
}