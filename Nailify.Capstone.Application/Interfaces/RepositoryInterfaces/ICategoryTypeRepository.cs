using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface ICategoryTypeRepository : IGenericRepository<CategoryType>
    {
        Task<CategoryType?> GetCategoryTypeWithCategoriesAsync(int categoryTypeId);
    }
}
