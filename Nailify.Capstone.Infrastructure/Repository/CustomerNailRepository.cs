using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Infrastructure.DBContext;

namespace Nailify.Capstone.Infrastructure.Repository
{
    public class CustomerNailRepository : GenericRepository<CustomerNail>, ICustomerNailRepository
    {
        public CustomerNailRepository(NailifyDbContext context) : base(context)
        {
        }

        public async Task<PagedList<CustomerNail>> GetPagedCustomerNailsAsync(int pageNumber, int pageSize, Guid? userId = null, string? name = null, bool? isPublic = null, bool? isFavorite = null)
        {
            var query = BuildCustomerNailQuery();

            if (userId.HasValue)
            {
                query = query.Where(nail => nail.UserId == userId.Value);
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                var normalizedName = name.Trim().ToLower();
                query = query.Where(nail => nail.Name.ToLower().Contains(normalizedName));
            }

            if (isPublic.HasValue)
            {
                query = query.Where(nail => nail.IsPublic == isPublic.Value);
            }

            if (isFavorite.HasValue)
            {
                query = query.Where(nail => nail.IsFavorite == isFavorite.Value);
            }

            var count = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedList<CustomerNail>(items, count, pageNumber, pageSize);
        }

        public async Task<CustomerNail?> GetCustomerNailDetailAsync(int customerNailId)
        {
            return await BuildCustomerNailQuery()
                .FirstOrDefaultAsync(nail => nail.CustomerNailId == customerNailId);
        }

        private IQueryable<CustomerNail> BuildCustomerNailQuery()
        {
            return _dbSet
                .Where(nail => nail.Status == "Active")
                .Include(nail => nail.NailShape)
                .Include(nail => nail.NailSurface)
                .Include(nail => nail.CustomerNailComponents)
                .ThenInclude(component => component.Component)
                .Include(nail => nail.CustomerNailComponents)
                .ThenInclude(component => component.CustomerComponent);
        }
    }
}
