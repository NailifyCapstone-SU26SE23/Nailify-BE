using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface IShapeMethodConfigRepository : IGenericRepository<ShapeMethodConfig>
    {
        Task<List<ShapeMethodConfig>> GetByNailShapeIdAsync(int nailShapeId, string? status = null);
        Task<PagedList<ShapeMethodConfig>> GetPagedShapeMethodConfigsAsync(int pageNumber, int pageSize, int? nailShapeId = null, string? name = null, string? status = null);
    }
}
