using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface IPromotionRepository : IGenericRepository<Promotion>
    {
        Task<PagedList<Promotion>> GetPagedPromotionsAsync(
            int pageNumber,
            int pageSize,
            PromotionType? type = null,
            PromotionScope? scope = null,
            DiscountType? discountType = null,
            DateTime? startDate = null,
            DateTime? endDate = null);
        Task<List<Promotion>> GetByCategoryIdAsync(int categoryId);
        Task<List<Promotion>> GetByCategoryTypeIdAsync(int categoryTypeId);
        Task<List<Promotion>> GetByNailDesignIdAsync(int nailDesignId);
        Task<List<Promotion>> GetActivePromotionsAsync(DateTime atUtc, IEnumerable<int>? selectedPromotionIds = null);
        Task<List<Promotion>> GetActivePromotionsForDisplayAsync(DateTime atUtc, PromotionType? type = null);
    }
}
