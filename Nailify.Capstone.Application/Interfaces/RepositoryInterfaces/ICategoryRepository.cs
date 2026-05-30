using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<List<Category>> GetCategoriesByTypeAsync(int categoryTypeId);
        Task<Category?> GetCategoryWithDesignsAsync(int categoryId);
    }
}
