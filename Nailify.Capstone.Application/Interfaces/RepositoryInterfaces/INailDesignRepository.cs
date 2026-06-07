using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface INailDesignRepository : IGenericRepository<NailDesign>
    {
        Task<List<NailDesign>> GetNailDesignsByCategoryAsync(int categoryId);
        Task<NailDesign?> GetNailDesignWithCategoriesAsync(int nailDesignId);
        Task<List<NailDesign>> GetActiveNailDesignsAsync();
        Task<PagedList<NailDesign>> GetPagedActiveNailDesignsAsync(
            int pageNumber,
            int pageSize,
            string? name = null,
            IEnumerable<int>? categoryIds = null);
    }
}
