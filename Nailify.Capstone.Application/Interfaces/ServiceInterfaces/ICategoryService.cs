using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.CategoryRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface ICategoryService
    {
        Task<ApiResult<PagedList<CategoryDto>>> GetPagedCategoriesAsync(
            int pageNumber,
            int pageSize,
            string? name = null,
            int? categoryTypeId = null);
        Task<ApiResult<CategoryDto>> GetCategoryByIdAsync(int id);
        Task<ApiResult<CategoryDto>> CreateCategoryAsync(CategoryCreateRequest request);
        Task<ApiResult<CategoryDto>> UpdateCategoryAsync(int id, CategoryUpdateRequest request);
        Task<ApiResult<bool>> DeleteCategoryAsync(int id);
    }
}
