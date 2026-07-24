using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.ShapeMethodConfigRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface IShapeMethodConfigService
    {
        Task<ApiResult<PagedList<ShapeMethodConfigDto>>> GetPagedShapeMethodConfigsAsync(int pageNumber, int pageSize, int? nailShapeId = null, string? name = null);
        Task<ApiResult<ShapeMethodConfigDto>> GetShapeMethodConfigByIdAsync(int id);
        Task<ApiResult<List<ShapeMethodConfigDto>>> GetShapeMethodConfigsByNailShapeIdAsync(int nailShapeId);
        Task<ApiResult<ShapeMethodConfigDto>> CreateShapeMethodConfigAsync(ShapeMethodConfigCreateRequest request);
        Task<ApiResult<ShapeMethodConfigDto>> UpdateShapeMethodConfigAsync(int id, ShapeMethodConfigUpdateRequest request);
        Task<ApiResult<bool>> DeleteShapeMethodConfigAsync(int id);
    }
}
