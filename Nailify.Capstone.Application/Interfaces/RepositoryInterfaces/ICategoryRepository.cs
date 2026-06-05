using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Application.Common;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<Category?> GetCategoryWithDesignsAsync(int categoryId);
        Task<PagedList<Category>> GetPagedCategoriesAsync(
            int pageNumber,
            int pageSize,
            string? name = null,
            int? categoryTypeId = null);
    }
}
