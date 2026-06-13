using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.Common;
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

        public async Task<Category?> GetCategoryWithDesignsAsync(int categoryId)
        {
            return await _dbSet
                .Include(c => c.CategoryType)
                .Include(c => c.NailCategories)
                .ThenInclude(nc => nc.NailDesign)
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId && c.Status == "Active");
        }

        public async Task<PagedList<Category>> GetPagedCategoriesAsync(
            int pageNumber,
            int pageSize,
            string? name = null,
            int? categoryTypeId = null)
        {
            var query = _dbSet
                .Include(c => c.CategoryType)
                .Where(c => c.Status == "Active");

            if (!string.IsNullOrWhiteSpace(name))
            {
                var normalizedName = name.Trim().ToLower();
                query = query.Where(c => c.Name.ToLower().Contains(normalizedName));
            }

            if (categoryTypeId.HasValue && categoryTypeId.Value > 0)
            {
                query = query.Where(c => c.CategoryTypeId == categoryTypeId.Value);
            }

            var count = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedList<Category>(items, count, pageNumber, pageSize);
        }
    }
}
