using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface IUserPromotionUsageRepository : IGenericRepository<UserPromotionUsage>
    {
        Task<UserPromotionUsage?> GetByUserAndPromotionAsync(Guid userId, int promotionId);
        /// <summary>
        /// Lấy danh sách tất cả các voucher hợp lệ của người dùng.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<UserPromotionUsage>> GetValidUserVouchersAsync(Guid userId);
    }
}
