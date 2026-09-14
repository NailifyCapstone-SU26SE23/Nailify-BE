using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Infrastructure.DBContext;

namespace Nailify.Capstone.Infrastructure.Repository
{
    public class ShapeMethodConfigRepository : GenericRepository<ShapeMethodConfig>, IShapeMethodConfigRepository
    {
        public ShapeMethodConfigRepository(NailifyDbContext context) : base(context)
        {
        }

        public async Task<List<ShapeMethodConfig>> GetByNailShapeIdAsync(int nailShapeId, string? status = null)
        {
            var query = _dbSet
                .Include(config => config.NailShape)
                .Where(config => config.NailShapeId == nailShapeId);
            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(config => config.Status == status);
            }

            return await query
                .OrderBy(config => config.Name)
                .ToListAsync();
        }

        public async Task<PagedList<ShapeMethodConfig>> GetPagedShapeMethodConfigsAsync(int pageNumber, int pageSize, int? nailShapeId = null, string? name = null, string? status = null)
        {
            var query = _dbSet
                .Include(config => config.NailShape)
                .AsQueryable();
            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(config => config.Status == status);
            }

            if (nailShapeId.HasValue)
            {
                query = query.Where(config => config.NailShapeId == nailShapeId.Value);
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                var normalizedName = name.Trim().ToLower();
                query = query.Where(config => config.Name.ToLower().Contains(normalizedName));
            }

            var count = await query.CountAsync();
            var items = await query
                .OrderBy(config => config.NailShapeId)
                .ThenBy(config => config.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedList<ShapeMethodConfig>(items, count, pageNumber, pageSize);
        }
    }
}
