using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.CategoryTypeRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface ICategoryTypeService
    {
        Task<ApiResult<List<CategoryTypeDto>>> GetAllCategoryTypesAsync();
        Task<ApiResult<CategoryTypeDto>> GetCategoryTypeByIdAsync(int id);
        Task<ApiResult<CategoryTypeDto>> CreateCategoryTypeAsync(CategoryTypeCreateRequest request);
        Task<ApiResult<CategoryTypeDto>> UpdateCategoryTypeAsync(CategoryTypeUpdateRequest request);
        Task<ApiResult<bool>> DeleteCategoryTypeAsync(int id);
    }
}
