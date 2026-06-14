using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Infrastructure.DBContext;

namespace Nailify.Capstone.Infrastructure.Repository
{
    public class NailSurfaceRepository : GenericRepository<NailSurface>, INailSurfaceRepository
    {
        public NailSurfaceRepository(NailifyDbContext context) : base(context)
        {
        }

        public async Task<List<NailSurface>> GetAllNailSurfacesAsync()
        {
            return await _dbSet
                .Where(ns => ns.Status == "Active")
                .ToListAsync();
        }

        public async Task<PagedList<NailSurface>> GetPagedNailSurfacesAsync(int pageNumber, int pageSize, string? name = null)
        {
            var query = _dbSet
                .Where(ns => ns.Status == "Active")
                .AsQueryable();
            if (!string.IsNullOrWhiteSpace(name))
            {
                var normalizedName = name.Trim().ToLower();
                query = query.Where(ns => ns.Name.ToLower().Contains(normalizedName));
            }

            var count = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedList<NailSurface>(items, count, pageNumber, pageSize);
        }
    }
}
