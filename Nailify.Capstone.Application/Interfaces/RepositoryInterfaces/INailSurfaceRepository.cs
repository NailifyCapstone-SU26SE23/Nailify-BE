using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Application.Common;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface INailSurfaceRepository : IGenericRepository<NailSurface>
    {
        Task<List<NailSurface>> GetAllNailSurfacesAsync();
        Task<PagedList<NailSurface>> GetPagedNailSurfacesAsync(int pageNumber, int pageSize, string? name = null);
    }
}
