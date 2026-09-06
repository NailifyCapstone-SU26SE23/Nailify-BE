using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface IFavoriteNailRepository : IGenericRepository<FavoriteNail>
    {
        Task<FavoriteNail?> GetByIdForUserAsync(int id, Guid userId);
        Task<FavoriteNail?> GetTrackedByIdForUserAsync(int id, Guid userId);
        Task<PagedList<FavoriteNail>> GetPagedByUserAsync(Guid userId, int pageNumber, int pageSize);
        Task<List<FavoriteNail>> GetFavoritesWithDetailsAsync(Guid userId);
        Task<List<FavoriteNail>> GetFavoritesByDesignAndVariantIdsAsync(Guid userId, IEnumerable<int> designIds, IEnumerable<int> variantIds);
        Task<List<FavoriteNail>> GetFavoritesByVariantIdsAsync(Guid userId, IEnumerable<int> variantIds);
    }
}
