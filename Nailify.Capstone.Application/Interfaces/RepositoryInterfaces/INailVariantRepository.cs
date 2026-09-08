using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface INailVariantRepository : IGenericRepository<NailVariant>
    {
        Task<List<NailVariant>> GetAllNailVariantsAsync();
        Task<PagedList<NailVariant>> GetPagedNailVariantsAsync(int pageNumber, int pageSize, int? nailDesignId = null, string? name = null);
        Task<List<NailVariant>> GetNailVariantsByDesignIdAsync(int nailDesignId);
        Task<List<NailVariant>> GetNailVariantsByIdsAsync(IEnumerable<int> nailVariantIds);
        Task<NailVariant?> GetNailVariantDetailAsync(int nailVariantId);
        Task<NailSummaryDto?> GetNailVariantSummaryAsync(int nailVariantId);
        List<int> GetDistinctVariantIdsAsync(IEnumerable<BookingItem> items);
        Task<List<NailVariant>> GetNailVariantsCapableByArtistAsync(Guid artistId);
    }
}
