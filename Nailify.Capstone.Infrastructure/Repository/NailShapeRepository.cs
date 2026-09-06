using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Infrastructure.DBContext;

namespace Nailify.Capstone.Infrastructure.Repository
{
    public class NailShapeRepository : GenericRepository<NailShape>, INailShapeRepository
    {
        public NailShapeRepository(NailifyDbContext context) : base(context)
        {
        }

        public async Task<List<NailShape>> GetAllNailShapesAsync()
        {
            return await _dbSet
                .Where(ns => ns.Status == "Active")
                .ToListAsync();
        }

        public async Task<PagedList<NailShape>> GetPagedNailShapesAsync(int pageNumber, int pageSize, string? name = null)
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

            return new PagedList<NailShape>(items, count, pageNumber, pageSize);
        }
    }
}
