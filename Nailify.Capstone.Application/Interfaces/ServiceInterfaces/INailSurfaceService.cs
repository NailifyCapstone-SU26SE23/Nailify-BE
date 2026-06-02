using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailSurfaceRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface INailSurfaceService
    {
        Task<ApiResult<PagedList<NailSurfaceDto>>> GetPagedNailSurfacesAsync(int pageNumber, int pageSize, string? name = null);
        Task<ApiResult<NailSurfaceDto>> GetNailSurfaceByIdAsync(int id);
        Task<ApiResult<NailSurfaceDto>> CreateNailSurfaceAsync(NailSurfaceCreateRequest request);
        Task<ApiResult<NailSurfaceDto>> UpdateNailSurfaceAsync(NailSurfaceUpdateRequest request);
        Task<ApiResult<bool>> DeleteNailSurfaceAsync(int id);
    }
}
