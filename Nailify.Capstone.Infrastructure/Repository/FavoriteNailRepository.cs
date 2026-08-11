using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Infrastructure.DBContext;

namespace Nailify.Capstone.Infrastructure.Repository
{
    public class FavoriteNailRepository : GenericRepository<FavoriteNail>, IFavoriteNailRepository
    {
        public FavoriteNailRepository(NailifyDbContext context) : base(context)
        {
        }

        public Task<FavoriteNail?> GetByIdForUserAsync(int id, Guid userId)
            => BuildQuery().FirstOrDefaultAsync(f => f.FavoriteNailId == id && f.UserId == userId);

        public Task<FavoriteNail?> GetTrackedByIdForUserAsync(int id, Guid userId)
            => _dbSet.FirstOrDefaultAsync(f => f.FavoriteNailId == id && f.UserId == userId);

        public async Task<PagedList<FavoriteNail>> GetPagedByUserAsync(Guid userId, int pageNumber, int pageSize)
        {
            var query = BuildQuery()
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.CreatedAt);
            var count = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedList<FavoriteNail>(items, count, pageNumber, pageSize);
        }
        public Task<List<FavoriteNail>> GetFavoritesWithDetailsAsync(Guid userId)
            => BuildQuery().Where(f => f.UserId == userId).ToListAsync();

        public async Task<List<FavoriteNail>> GetFavoritesByDesignAndVariantIdsAsync(Guid userId, IEnumerable<int> designIds, IEnumerable<int> variantIds)
        {
            var designIdList = designIds as IReadOnlyCollection<int> ?? designIds.ToList();
            var variantIdList = variantIds as IReadOnlyCollection<int> ?? variantIds.ToList();

            return await _dbSet.AsNoTracking()
                .Where(f => f.UserId == userId &&
                    ((f.NailVariantId == null && f.NailDesignId != null && designIdList.Contains(f.NailDesignId.Value)) ||
                     (f.NailVariantId != null && variantIdList.Contains(f.NailVariantId.Value))))
                .ToListAsync();
        }

        public async Task<List<FavoriteNail>> GetFavoritesByVariantIdsAsync(Guid userId, IEnumerable<int> variantIds)
        {
            var variantIdList = variantIds as IReadOnlyCollection<int> ?? variantIds.ToList();

            return await _dbSet.AsNoTracking()
                .Where(f => f.UserId == userId &&
                            f.NailVariantId != null &&
                            variantIdList.Contains(f.NailVariantId.Value))
                .ToListAsync();
        }

        private IQueryable<FavoriteNail> BuildQuery()
            => _dbSet
                .AsNoTracking()
                .Include(f => f.NailDesign)
                    .ThenInclude(d => d!.NailCategories)
                        .ThenInclude(nc => nc.Category)
                            .ThenInclude(c => c.CategoryType)
                .Include(f => f.NailVariant)
                    .ThenInclude(v => v!.NailShape)
                .Include(f => f.NailVariant)
                    .ThenInclude(v => v!.NailSurface);
    }
}
