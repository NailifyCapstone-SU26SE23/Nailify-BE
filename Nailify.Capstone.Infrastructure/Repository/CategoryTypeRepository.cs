using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.Common;
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
                .FirstOrDefaultAsync(ct => ct.CategoryTypeId == categoryTypeId && ct.Status == "Active");
        }

        public async Task<PagedList<CategoryType>> GetPagedCategoryTypesAsync(
            int pageNumber,
            int pageSize,
            string? name = null)
        {
            var query = _dbSet
                .Include(ct => ct.Categories)
                .Where(ct => ct.Status == "Active");

            if (!string.IsNullOrWhiteSpace(name))
            {
                var normalizedName = name.Trim().ToLower();
                query = query.Where(ct => ct.Name.ToLower().Contains(normalizedName));
            }

            var count = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedList<CategoryType>(items, count, pageNumber, pageSize);
        }
    }
}
