using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailCategoryRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface INailCategoryService
    {
        Task<ApiResult<List<NailCategoryDto>>> GetByNailDesignIdAsync(int nailDesignId);
        Task<ApiResult<List<NailCategoryDto>>> AssignCategoriesToNailDesignAsync(int nailDesignId, List<NailCategoryRequest> request);
        Task<ApiResult<List<NailCategoryDto>>> DeleteByNailDesignIdAsync(int nailDesignId);
    }
}
