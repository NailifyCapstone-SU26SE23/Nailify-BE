using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailVariantRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface INailVariantService
    {
        Task<ApiResult<PagedList<NailVariantDto>>> GetPagedNailVariantsAsync(int pageNumber, int pageSize, int? nailDesignId = null, string? name = null, Guid? userId = null);
        Task<ApiResult<NailVariantDto>> GetNailVariantByIdAsync(int id, Guid? userId = null);
        Task<ApiResult<NailVariantDto>> CreateNailVariantAsync(NailVariantCreateRequest request, string? imageUrl = null);
        Task<ApiResult<NailVariantDto>> UpdateNailVariantAsync(int id, NailVariantUpdateRequest request, string? imageUrl = null);
        Task<ApiResult<bool>> DeleteNailVariantAsync(int id);
        Task<ApiResult<List<NailVariantDto>>> GetCapableNailVariantsAsync(Guid artistId, Guid? userId = null);
    }
}
