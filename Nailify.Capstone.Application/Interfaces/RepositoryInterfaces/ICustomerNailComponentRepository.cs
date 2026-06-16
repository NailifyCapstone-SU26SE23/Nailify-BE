using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface ICustomerNailComponentRepository : IGenericRepository<CustomerNailComponent>
    {
        Task<PagedList<CustomerNailComponent>> GetPagedCustomerNailComponentsAsync(int pageNumber, int pageSize, int? customerNailId = null);
        Task<CustomerNailComponent?> GetCustomerNailComponentDetailAsync(int customerNailComponentId);
    }
}
