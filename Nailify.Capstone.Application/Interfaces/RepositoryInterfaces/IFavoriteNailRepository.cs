using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface IFavoriteNailRepository : IGenericRepository<FavoriteNail>
    {
        Task<FavoriteNail?> GetByIdForUserAsync(int id, Guid userId);
        Task<FavoriteNail?> GetTrackedByIdForUserAsync(int id, Guid userId);
        Task<PagedList<FavoriteNail>> GetPagedByUserAsync(Guid userId, int pageNumber, int pageSize);
        Task<List<FavoriteNail>> GetAllWithVariantAsync();
        Task<Dictionary<int, int>> GetFavoriteCountsByVariantAstbc();
        Task<int> CountFavoritesWithVariantByUserIdAsync(Guid userId);
    }
}
