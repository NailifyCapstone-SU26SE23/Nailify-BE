using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailShapeRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface INailShapeService
    {
        Task<ApiResult<PagedList<NailShapeDto>>> GetPagedNailShapesAsync(int pageNumber, int pageSize, string? name = null);
        Task<ApiResult<NailShapeDto>> GetNailShapeByIdAsync(int id);
        Task<ApiResult<NailShapeDto>> CreateNailShapeAsync(NailShapeCreateRequest request, string? imageUrl = null);
        Task<ApiResult<NailShapeDto>> UpdateNailShapeAsync(NailShapeUpdateRequest request);
        Task<ApiResult<bool>> DeleteNailShapeAsync(int id);
    }
}
