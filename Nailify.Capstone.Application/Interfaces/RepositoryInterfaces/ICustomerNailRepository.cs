using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface ICustomerNailRepository : IGenericRepository<CustomerNail>
    {
        Task<PagedList<CustomerNail>> GetPagedCustomerNailsAsync(int pageNumber, int pageSize, Guid? userId = null, string? name = null);
        Task<CustomerNail?> GetCustomerNailDetailAsync(int customerNailId);
    }
}
