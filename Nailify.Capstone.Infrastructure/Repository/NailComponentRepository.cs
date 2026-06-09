using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Infrastructure.DBContext;

namespace Nailify.Capstone.Infrastructure.Repository
{
    public class NailComponentRepository : GenericRepository<NailComponent>, INailComponentRepository
    {
        public NailComponentRepository(NailifyDbContext context) : base(context)
        {
        }

        public async Task<List<NailComponent>> GetAllNailComponentsAsync()
        {
            return await BuildNailComponentQuery().ToListAsync();
        }

        public async Task<PagedList<NailComponent>> GetPagedNailComponentsAsync(int pageNumber, int pageSize)
        {
            var query = BuildNailComponentQuery();
            var count = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedList<NailComponent>(items, count, pageNumber, pageSize);
        }

        public async Task<NailComponent?> GetNailComponentDetailAsync(int nailComponentId)
        {
            return await BuildNailComponentQuery()
                .FirstOrDefaultAsync(nc => nc.NailComponentId == nailComponentId);
        }

        private IQueryable<NailComponent> BuildNailComponentQuery()
        {
            return _dbSet.Include(nc => nc.Component);
        }
    }
}
