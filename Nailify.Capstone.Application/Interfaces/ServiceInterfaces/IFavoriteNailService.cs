using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.FavoriteNailRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface IFavoriteNailService
    {
        Task<ApiResult<PagedList<FavoriteNailDto>>> GetPagedAsync(Guid userId, int pageNumber, int pageSize);
        Task<ApiResult<FavoriteNailDto>> GetByIdAsync(Guid userId, int id);
        Task<ApiResult<FavoriteNailDto>> CreateAsync(Guid userId, FavoriteNailRequest request);
        Task<ApiResult<FavoriteNailDto>> UpdateAsync(Guid userId, int id, FavoriteNailRequest request);
        Task<ApiResult<bool>> DeleteAsync(Guid userId, int id);
    }
}
