using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Infrastructure.DBContext;

namespace Nailify.Capstone.Infrastructure.Repository
{
    public class NailCategoryRepository : GenericRepository<NailCategory>, INailCategoryRepository
    {
        public NailCategoryRepository(NailifyDbContext context) : base(context)
        {
        }

        public async Task<List<NailCategory>> GetByNailDesignIdAsync(int nailDesignId)
        {
            return await _dbSet
                .Include(nc => nc.Category)
                .ThenInclude(category => category.CategoryType)
                .Where(nc => nc.NailDesignId == nailDesignId)
                .ToListAsync();
        }
    }
}
