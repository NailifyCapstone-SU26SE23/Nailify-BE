using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Infrastructure.DBContext;

namespace Nailify.Capstone.Infrastructure.Repository
{
    public class CategoryTypeRepository : GenericRepository<CategoryType>, ICategoryTypeRepository
    {
        public CategoryTypeRepository(NailifyDbContext context) : base(context)
        {
        }

        public async Task<CategoryType?> GetCategoryTypeWithCategoriesAsync(int categoryTypeId)
        {
            return await _dbSet
                .Include(ct => ct.Categories)
                .FirstOrDefaultAsync(ct => ct.CategoryTypeId == categoryTypeId);
        }
    }
}
