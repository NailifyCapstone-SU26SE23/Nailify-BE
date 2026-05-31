using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.CategoryRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface ICategoryService
    {
        Task<ApiResult<List<CategoryDto>>> GetAllCategoriesAsync();
        Task<ApiResult<CategoryDto>> GetCategoryByIdAsync(int id);
        Task<ApiResult<CategoryDto>> CreateCategoryAsync(CategoryCreateRequest request);
        Task<ApiResult<CategoryDto>> UpdateCategoryAsync(CategoryUpdateRequest request);
        Task<ApiResult<bool>> DeleteCategoryAsync(int id);
        Task<ApiResult<List<CategoryDto>>> GetCategoriesByTypeAsync(int categoryTypeId);
    }
}
