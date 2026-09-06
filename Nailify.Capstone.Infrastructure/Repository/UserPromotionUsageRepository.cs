using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Infrastructure.DBContext;

namespace Nailify.Capstone.Infrastructure.Repository
{
    public class UserPromotionUsageRepository : GenericRepository<UserPromotionUsage>, IUserPromotionUsageRepository
    {
        public UserPromotionUsageRepository(NailifyDbContext context) : base(context)
        {
        }

        public async Task<UserPromotionUsage?> GetByUserAndPromotionAsync(Guid userId, int promotionId)
        {
            return await _dbSet.FirstOrDefaultAsync(usage =>
                usage.UserId == userId &&
                usage.PromotionId == promotionId);
        }

        public async Task<List<UserPromotionUsage>> GetValidUserVouchersAsync(Guid userId)
        {
            return await _dbSet
                            .AsNoTracking()
                            .Include(u => u.Promotion)
                            .Where(u => u.UserId == userId && (u.ReceivedCount ?? 0) > u.UsageCount)
                            .ToListAsync();
        }
    }
}
