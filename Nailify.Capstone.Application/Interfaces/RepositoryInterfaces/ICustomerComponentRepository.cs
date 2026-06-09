using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface ICustomerComponentRepository : IGenericRepository<CustomerComponent>
    {
        Task<PagedList<CustomerComponent>> GetPagedCustomerComponentsAsync(int pageNumber, int pageSize, Guid? userId = null, string? name = null, ComponentType? componentType = null);
        Task<List<int>> GetCustomerNailIdsByCustomerComponentIdAsync(int customerComponentId);
    }
}
