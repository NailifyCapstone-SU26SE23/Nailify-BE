using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Application.Common;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface IComponentRepository : IGenericRepository<Component>
    {
        Task<List<Component>> GetAllComponentsAsync();
        Task<PagedList<Component>> GetPagedComponentsAsync(int pageNumber, int pageSize, string? name = null, ComponentType? componentType = null);
    }
}
