using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Infrastructure.DBContext;

namespace Nailify.Capstone.Infrastructure.Repository
{
    public class ComponentRepository : GenericRepository<Component>, IComponentRepository
    {
        public ComponentRepository(NailifyDbContext context) : base(context)
        {
        }

        public async Task<List<Component>> GetAllComponentsAsync()
        {
            return await _dbSet
                .Where(component => component.Status == "Active")
                .ToListAsync();
        }

        public async Task<PagedList<Component>> GetPagedComponentsAsync(int pageNumber, int pageSize, string? name = null, ComponentType? componentType = null)
        {
            var query = _dbSet
                .Where(component => component.Status == "Active")
                .AsQueryable();
            if (!string.IsNullOrWhiteSpace(name))
            {
                var normalizedName = name.Trim().ToLower();
                query = query.Where(c => c.Name.ToLower().Contains(normalizedName));
            }

            if (componentType.HasValue)
            {
                query = query.Where(c => c.ComponentType == componentType.Value);
            }

            var count = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedList<Component>(items, count, pageNumber, pageSize);
        }
    }
}
