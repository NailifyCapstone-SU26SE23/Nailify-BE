using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailDesignRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface INailDesignService
    {
        Task<ApiResult<PagedList<NailDesignDto>>> GetPagedNailDesignsAsync(
            int pageNumber,
            int pageSize,
            string? name = null,
            IEnumerable<int>? categoryIds = null,
            Guid? userId = null);
        Task<ApiResult<NailDesignDto>> GetNailDesignByIdAsync(int id, Guid? userId = null);
        Task<ApiResult<NailDesignDto>> CreateNailDesignAsync(NailDesignCreateRequest request, string? imageUrl = null);
        Task<ApiResult<NailDesignDto>> UpdateNailDesignAsync(int id, NailDesignUpdateRequest request, string? newImageUrl = null);
        Task<ApiResult<bool>> DeleteNailDesignAsync(int id);
        Task<ApiResult<List<NailDesignDto>>> GetNailDesignsByCategoryAsync(int categoryId, Guid? userId = null);
    }
}
