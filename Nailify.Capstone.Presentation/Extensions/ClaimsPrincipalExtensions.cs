using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Nailify.Capstone.Presentation.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("Không tìm thấy thông tin định danh người dùng trong Token.");
            }
            return Guid.Parse(userIdClaim);
        }
    }
}
