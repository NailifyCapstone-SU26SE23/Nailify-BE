using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Application.Common;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface INailShapeRepository : IGenericRepository<NailShape>
    {
        Task<List<NailShape>> GetAllNailShapesAsync();
        Task<PagedList<NailShape>> GetPagedNailShapesAsync(int pageNumber, int pageSize, string? name = null);
    }
}
