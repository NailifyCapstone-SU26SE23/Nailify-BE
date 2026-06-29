using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface IUserPromotionUsageRepository : IGenericRepository<UserPromotionUsage>
    {
        Task<UserPromotionUsage?> GetByUserAndPromotionAsync(Guid userId, int promotionId);
    }
}
