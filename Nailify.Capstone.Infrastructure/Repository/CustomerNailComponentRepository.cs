using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Infrastructure.DBContext;

namespace Nailify.Capstone.Infrastructure.Repository
{
    public class CustomerNailComponentRepository : GenericRepository<CustomerNailComponent>, ICustomerNailComponentRepository
    {
        public CustomerNailComponentRepository(NailifyDbContext context) : base(context)
        {
        }

        public async Task<PagedList<CustomerNailComponent>> GetPagedCustomerNailComponentsAsync(int pageNumber, int pageSize, int? customerNailId = null)
        {
            var query = BuildCustomerNailComponentQuery();

            if (customerNailId.HasValue)
            {
                query = query.Where(component => component.CustomerNailId == customerNailId.Value);
            }

            var count = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedList<CustomerNailComponent>(items, count, pageNumber, pageSize);
        }

        public async Task<CustomerNailComponent?> GetCustomerNailComponentDetailAsync(int customerNailComponentId)
        {
            return await BuildCustomerNailComponentQuery()
                .FirstOrDefaultAsync(component => component.CustomerNailComponentId == customerNailComponentId);
        }

        public async Task<List<CustomerNailComponent>> GetByCustomerNailIdAsync(int customerNailId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(component => component.Component)
                .Include(component => component.CustomerComponent)
                .Where(component => component.CustomerNailId == customerNailId)
                .ToListAsync();
        }

        public async Task<List<int>> GetCustomerNailIdsByComponentIdAsync(int componentId)
        {
            return await _dbSet
                .Where(component => component.ComponentId == componentId)
                .Select(component => component.CustomerNailId)
                .Distinct()
                .ToListAsync();
        }

        private IQueryable<CustomerNailComponent> BuildCustomerNailComponentQuery()
        {
            return _dbSet
                .Include(component => component.Component)
                .Include(component => component.CustomerComponent);
        }
    }
}
