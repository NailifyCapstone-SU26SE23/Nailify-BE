using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface IGoogleAuthService
    {
        Task<GoogleUserInfoDTO?> VerifyTokenAsync(string idToken);
    }
}
