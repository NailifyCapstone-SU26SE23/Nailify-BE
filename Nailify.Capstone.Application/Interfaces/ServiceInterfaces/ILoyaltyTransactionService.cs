using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.LoyaltyTransactionRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface ILoyaltyTransactionService
    {
        Task<ApiResult<PagedList<LoyaltyTransactionDto>>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Guid? userId = null);
        Task<ApiResult<LoyaltyTransactionDto>> GetByIdAsync(int id);
        Task<ApiResult<LoyaltyTransactionDto>> UpdateAsync(int id, LoyaltyTransactionUpdateRequest request);
    }
}
