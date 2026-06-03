using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Infrastructure.DBContext;

namespace Nailify.Capstone.Infrastructure.Repository
{
    public class CustomerComponentRepository : GenericRepository<CustomerComponent>, ICustomerComponentRepository
    {
        public CustomerComponentRepository(NailifyDbContext context) : base(context)
        {
        }

        public async Task<PagedList<CustomerComponent>> GetPagedCustomerComponentsAsync(int pageNumber, int pageSize, Guid? userId = null, string? name = null, ComponentType? componentType = null)
        {
            var query = _dbSet.AsQueryable();

            if (userId.HasValue)
            {
                query = query.Where(component => component.UserId == userId.Value);
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                var normalizedName = name.Trim().ToLower();
                query = query.Where(component => component.Name.ToLower().Contains(normalizedName));
            }

            if (componentType.HasValue)
            {
                query = query.Where(component => component.ComponentType == componentType.Value);
            }

            var count = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedList<CustomerComponent>(items, count, pageNumber, pageSize);
        }

        public async Task<List<int>> GetCustomerNailIdsByCustomerComponentIdAsync(int customerComponentId)
        {
            return await _context.CustomerNailComponents
                .Where(component => component.CustomerComponentId == customerComponentId)
                .Select(component => component.CustomerNailId)
                .Distinct()
                .ToListAsync();
        }
    }
}
