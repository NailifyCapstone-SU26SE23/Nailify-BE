using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface IShapeMethodConfigRepository : IGenericRepository<ShapeMethodConfig>
    {
        Task<List<ShapeMethodConfig>> GetActiveByNailShapeIdAsync(int nailShapeId);
        Task<PagedList<ShapeMethodConfig>> GetPagedShapeMethodConfigsAsync(int pageNumber, int pageSize, int? nailShapeId = null, string? name = null);
    }
}
