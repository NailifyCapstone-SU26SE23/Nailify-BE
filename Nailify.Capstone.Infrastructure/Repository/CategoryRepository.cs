using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Infrastructure.DBContext;

namespace Nailify.Capstone.Infrastructure.Repository
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(NailifyDbContext context) : base(context)
        {
        }

        public async Task<List<Category>> GetCategoriesByTypeAsync(int categoryTypeId)
        {
            return await _dbSet
                .Where(c => c.CategoryTypeId == categoryTypeId && c.Status == "Active")
                .Include(c => c.CategoryType)
                .ToListAsync();
        }

        public async Task<Category?> GetCategoryWithDesignsAsync(int categoryId)
        {
            return await _dbSet
                .Include(c => c.CategoryType)
                .Include(c => c.NailCategories)
                .ThenInclude(nc => nc.NailDesign)
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId);
        }
    }
}
