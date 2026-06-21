using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Infrastructure.DBContext;

namespace Nailify.Capstone.Infrastructure.Repository
{
    public class LoyaltyTransactionRepository : GenericRepository<LoyaltyTransaction>, ILoyaltyTransactionRepository
    {
        public LoyaltyTransactionRepository(NailifyDbContext context) : base(context) { }

        public async Task<PagedList<LoyaltyTransaction>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Guid? userId = null)
        {
            var query = _dbSet.AsNoTracking();
            if (userId.HasValue)
            {
                query = query.Where(transaction => transaction.CustomerId == userId.Value);
            }

            var totalItems = await query.CountAsync();
            var items = await query
                .OrderByDescending(transaction => transaction.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedList<LoyaltyTransaction>(items, totalItems, pageNumber, pageSize);
        }
    }
}
