using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailComponentRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface INailComponentService
    {
        Task<ApiResult<PagedList<NailComponentDto>>> GetPagedNailComponentsAsync(int pageNumber, int pageSize);
        Task<ApiResult<NailComponentDto>> GetNailComponentByIdAsync(int id);
        Task<ApiResult<NailComponentDto>> CreateNailComponentAsync(NailComponentCreateRequest request);
        Task<ApiResult<NailComponentDto>> UpdateNailComponentAsync(NailComponentUpdateRequest request);
        Task<ApiResult<bool>> DeleteNailComponentAsync(int id);
    }
}
