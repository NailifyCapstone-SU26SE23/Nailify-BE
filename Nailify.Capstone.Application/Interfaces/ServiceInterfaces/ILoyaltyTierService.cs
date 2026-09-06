using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.LoyaltyTierRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface ILoyaltyTierService
    {
        Task<ApiResult<List<LoyaltyTierDto>>> GetAllAsync();
        Task<ApiResult<LoyaltyTierDto>> GetByIdAsync(int id);
        Task<ApiResult<UserLoyaltyDto>> GetMyLoyaltyAsync(Guid userId);
        Task<ApiResult<LoyaltyTierDto>> CreateAsync(LoyaltyTierRequest request, string? imageUrl = null);
        Task<ApiResult<LoyaltyTierDto>> UpdateAsync(int id, LoyaltyTierRequest request, string? imageUrl = null);
        Task<ApiResult<bool>> DeleteAsync(int id);
    }
}
