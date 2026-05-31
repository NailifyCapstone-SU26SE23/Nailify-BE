using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailDesignRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface INailDesignService
    {
        Task<ApiResult<List<NailDesignDto>>> GetAllNailDesignsAsync();
        Task<ApiResult<PagedList<NailDesignDto>>> GetPagedNailDesignsAsync(int pageNumber, int pageSize);
        Task<ApiResult<NailDesignDto>> GetNailDesignByIdAsync(int id);
        Task<ApiResult<NailDesignDto>> CreateNailDesignAsync(NailDesignCreateRequest request);
        Task<ApiResult<NailDesignDto>> UpdateNailDesignAsync(NailDesignUpdateRequest request);
        Task<ApiResult<bool>> DeleteNailDesignAsync(int id);
        Task<ApiResult<List<NailDesignDto>>> GetNailDesignsByCategoryAsync(int categoryId);
    }
}
