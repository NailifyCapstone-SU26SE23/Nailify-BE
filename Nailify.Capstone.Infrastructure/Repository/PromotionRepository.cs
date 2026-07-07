using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;
using Nailify.Capstone.Infrastructure.DBContext;

namespace Nailify.Capstone.Infrastructure.Repository
{
    public class PromotionRepository : GenericRepository<Promotion>, IPromotionRepository
    {
        public PromotionRepository(NailifyDbContext context) : base(context)
        {
        }

        public async Task<PagedList<Promotion>> GetPagedPromotionsAsync(
            int pageNumber,
            int pageSize,
            PromotionType? type = null,
            PromotionScope? scope = null,
            DiscountType? discountType = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var query = ApplyPromotionFilters(_dbSet.AsQueryable(), type, scope, discountType, startDate, endDate);
            var count = await query.CountAsync();
            var items = await query
                .OrderByDescending(promotion => promotion.StartDate)
                .ThenBy(promotion => promotion.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedList<Promotion>(items, count, pageNumber, pageSize);
        }

        public async Task<List<Promotion>> GetByCategoryIdAsync(int categoryId)
        {
            return await _dbSet
                .Where(promotion => promotion.CategoryId == categoryId)
                .OrderByDescending(promotion => promotion.StartDate)
                .ThenBy(promotion => promotion.Name)
                .ToListAsync();
        }

        public async Task<List<Promotion>> GetByCategoryTypeIdAsync(int categoryTypeId)
        {
            return await _dbSet
                .Where(promotion => promotion.CategoryTypeId == categoryTypeId)
                .OrderByDescending(promotion => promotion.StartDate)
                .ThenBy(promotion => promotion.Name)
                .ToListAsync();
        }

        public async Task<List<Promotion>> GetByNailDesignIdAsync(int nailDesignId)
        {
            return await _dbSet
                .Where(promotion => promotion.NailDesignId == nailDesignId)
                .OrderByDescending(promotion => promotion.StartDate)
                .ThenBy(promotion => promotion.Name)
                .ToListAsync();
        }

        public async Task<List<Promotion>> GetActivePromotionsAsync(DateTime atUtc, IEnumerable<int>? selectedPromotionIds = null)
        {
            var selectedIds = selectedPromotionIds?.Distinct().ToList() ?? new List<int>();

            var query = _dbSet
                .Where(p =>
                    p.Status == "Active" &&
                    p.StartDate <= atUtc &&
                    (!p.EndDate.HasValue || p.EndDate.Value >= atUtc) &&
                    (p.Scope == PromotionScope.FirstTimeUser ||
                     !p.UsageLimit.HasValue ||
                     p.CurrentUsageCount < p.UsageLimit.Value));

            if (selectedIds.Count > 0)
            {
                query = query.Where(p => !p.IsSelectable || selectedIds.Contains(p.PromotionId));
            }
            else
            {
                query = query.Where(p => !p.IsSelectable);
            }

            return await query.ToListAsync();
        }

        public async Task<List<Promotion>> GetActivePromotionsForDisplayAsync(DateTime atUtc, PromotionType? type = null)
        {
            var query = _dbSet
                .Where(promotion =>
                    promotion.Status == "Active" &&
                    promotion.StartDate <= atUtc &&
                    (!promotion.EndDate.HasValue || promotion.EndDate.Value >= atUtc) &&
                    (promotion.Scope == PromotionScope.FirstTimeUser ||
                     !promotion.UsageLimit.HasValue ||
                     promotion.CurrentUsageCount < promotion.UsageLimit.Value));

            if (type.HasValue)
            {
                query = query.Where(promotion => promotion.Type == type.Value);
            }

            return await query
                .OrderByDescending(promotion => promotion.StartDate)
                .ThenBy(promotion => promotion.Name)
                .ToListAsync();
        }

        private static IQueryable<Promotion> ApplyPromotionFilters(
            IQueryable<Promotion> query,
            PromotionType? type,
            PromotionScope? scope,
            DiscountType? discountType,
            DateTime? startDate,
            DateTime? endDate)
        {
            if (type.HasValue)
            {
                query = query.Where(promotion => promotion.Type == type.Value);
            }

            if (scope.HasValue)
            {
                query = query.Where(promotion => promotion.Scope == scope.Value);
            }

            if (discountType.HasValue)
            {
                query = query.Where(promotion => promotion.DiscountType == discountType.Value);
            }

            if (startDate.HasValue)
            {
                query = query.Where(promotion => promotion.StartDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(promotion => !promotion.EndDate.HasValue || promotion.EndDate.Value <= endDate.Value);
            }

            return query;
        }
    }
}
