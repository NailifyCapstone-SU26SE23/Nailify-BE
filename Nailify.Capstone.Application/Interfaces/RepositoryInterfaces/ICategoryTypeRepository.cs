using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Application.Common;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface ICategoryTypeRepository : IGenericRepository<CategoryType>
    {
        Task<CategoryType?> GetCategoryTypeWithCategoriesAsync(int categoryTypeId);
        Task<PagedList<CategoryType>> GetPagedCategoryTypesAsync(
            int pageNumber,
            int pageSize,
            string? name = null);
    }
}
