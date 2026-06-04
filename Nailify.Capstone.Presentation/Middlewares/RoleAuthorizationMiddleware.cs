using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Nailify.Capstone.Presentation.Middlewares
{
    public class RoleAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        public RoleAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint(); //tìm ednpoint

            if (endpoint == null)
            {
                await _next(context);
                return;
            }
            var roleAttribute = endpoint.Metadata.GetMetadata<HasRoleAttribute>(); //lấy metadata
            // Nếu KHÔNG gắn thẻ -> API này là PUBLIC, cho qua luôn
            if (roleAttribute == null)
            {
                await _next(context);
                return;
            }
            var user = context.User; // nếu có thẻ role -> check
            if (user.Identity == null || !user.Identity.IsAuthenticated)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    isSucceeded = false,
                    status = StatusCodes.Status401Unauthorized,
                    message = "Yêu cầu đăng nhập để truy cập chức năng này!"
                }));
                return;
            }

            var requiredRoles = roleAttribute.AllowedRoles;
            bool hasPermission = false;

            // Quét xem trong danh sách quyền được phép, người dùng hiện tại có sở hữu quyền nào không
            foreach (var role in requiredRoles)
            {
                if (user.IsInRole(role) ||
                    user.HasClaim(c => c.Type == "role" && c.Value.Equals(role, System.StringComparison.OrdinalIgnoreCase)) ||
                    user.HasClaim(c => c.Type == System.Security.Claims.ClaimTypes.Role && c.Value.Equals(role, System.StringComparison.OrdinalIgnoreCase)))
                {
                    hasPermission = true;
                    break; // Chỉ cần khớp 1 quyền là hợp lệ
                }
            }
            if (!hasPermission)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";

                var responseText = JsonSerializer.Serialize(new 
                {
                    isSucceeded = false,
                    status = StatusCodes.Status403Forbidden,
                    message = "Bạn không có quyền truy cập vào chức năng này!" });
                await context.Response.WriteAsync(responseText);
                return;
            }

            // Vượt qua tất cả chốt chặn thành công -> Cho phép thực thi hàm trong Controller
            await _next(context);

        }
    }
}
