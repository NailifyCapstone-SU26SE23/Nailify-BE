using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Infrastructure.DBContext;

namespace Nailify.Capstone.Infrastructure.Repository
{
    public class NailDesignRepository : GenericRepository<NailDesign>, INailDesignRepository
    {
        public NailDesignRepository(NailifyDbContext context) : base(context)
        {
        }

        public async Task<List<NailDesign>> GetNailDesignsByCategoryAsync(int categoryId)
        {
            return await BuildNailDesignQuery()
                .Where(nd => nd.NailCategories.Any(nc => nc.CategoryId == categoryId) && nd.Status == "Active")
                .ToListAsync();
        }

        public async Task<NailDesign?> GetNailDesignWithCategoriesAsync(int nailDesignId)
        {
            return await BuildNailDesignQuery()
                .FirstOrDefaultAsync(nd => nd.NailDesignId == nailDesignId && nd.Status == "Active");
        }

        public async Task<NailSummaryDto?> GetNailDesignSummaryAsync(int nailDesignId)
        {
            var exists = await _dbSet.AnyAsync(nd => nd.NailDesignId == nailDesignId && nd.Status == "Active");
            if (!exists)
            {
                return null;
            }

            var bookingIds = _context.BookingItems
                .Where(item => item.NailVariant != null && item.NailVariant.NailDesignId == nailDesignId)
                .Select(item => item.BookingId)
                .Distinct();

            var totalBookings = await bookingIds.CountAsync();
            var totalFavorites = await _context.FavoriteNails
                .CountAsync(favorite =>
                    favorite.NailDesignId == nailDesignId
                    || (favorite.NailVariant != null && favorite.NailVariant.NailDesignId == nailDesignId));
            var ratingQuery = _context.BookingRatings
                .Where(rating => rating.Status == "Active" && bookingIds.Contains(rating.BookingId));
            var ratingCount = await ratingQuery.CountAsync();
            var averageRating = await ratingQuery.AverageAsync(rating => (double?)rating.OverallScore);

            return new NailSummaryDto
            {
                TotalBookings = totalBookings,
                TotalFavorites = totalFavorites,
                AverageRating = averageRating.HasValue ? Math.Round(averageRating.Value, 2) : 0,
                RatingCount = ratingCount
            };
        }

        public async Task<List<NailDesign>> GetActiveNailDesignsAsync()
        {
            return await BuildNailDesignQuery()
                .Where(nd => nd.Status == "Active")
                .ToListAsync();
        }

        public async Task<PagedList<NailDesign>> GetPagedActiveNailDesignsAsync(
            int pageNumber,
            int pageSize,
            string? name = null,
            IEnumerable<int>? categoryIds = null)
        {
            var query = BuildNailDesignQuery().Where(nd => nd.Status == "Active");
            if (!string.IsNullOrWhiteSpace(name))
            {
                var normalizedName = name.Trim().ToLower();
                query = query.Where(nd => nd.Name.ToLower().Contains(normalizedName));
            }

            var filterCategoryIds = categoryIds?
                .Where(categoryId => categoryId > 0)
                .Distinct()
                .ToList();
            if (filterCategoryIds != null && filterCategoryIds.Any())
            {
                query = query.Where(nd =>
                    filterCategoryIds.All(categoryId =>
                        nd.NailCategories.Any(nc => nc.CategoryId == categoryId)));
            }

            var count = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedList<NailDesign>(items, count, pageNumber, pageSize);
        }

        private IQueryable<NailDesign> BuildNailDesignQuery()
        {
            return _dbSet
                .Include(nd => nd.NailCategories)
                .ThenInclude(nc => nc.Category)
                .ThenInclude(c => c.CategoryType)
                .Include(nd => nd.NailVariants)
                .ThenInclude(nv => nv.NailShape)
                .ThenInclude(ns => ns.ShapeMethodConfigs)
                .Include(nd => nd.NailVariants)
                .ThenInclude(nv => nv.NailShape)
                .Include(nd => nd.NailVariants)
                .ThenInclude(nv => nv.NailSurface)
                .Include(nd => nd.NailVariants)
                .ThenInclude(nv => nv.NailComponents)
                .ThenInclude(nc => nc.Component);
        }
    }
}
