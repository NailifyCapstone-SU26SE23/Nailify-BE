using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.AuthRequestDTOs;
using Nailify.Capstone.Application.DTOs.RequestDTOs.UserRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface IAuthService
    {
        Task<AuthResponse?> LoginAsync(LoginRequest request);
        Task<AuthResponse?> GoogleLoginAsync(GoogleLoginRequest request);
        Task<ApiResult<UserDto>> RegisterAsync(UserRegisterRequest request);
        Task<ApiResult<bool>> ForgotPasswordAsync(ForgotPasswordRequest request);
        Task<ApiResult<bool>> CheckResetPasswordTokenAsync(CheckResetPasswordTokenRequest request);
        Task<ApiResult<bool>> ResetPasswordAsync(ResetPasswordRequest request);
    }
}
