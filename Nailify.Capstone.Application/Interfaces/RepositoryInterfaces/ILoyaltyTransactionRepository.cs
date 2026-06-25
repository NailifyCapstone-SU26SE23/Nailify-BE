using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface ILoyaltyTransactionRepository : IGenericRepository<LoyaltyTransaction>
    {
        Task<PagedList<LoyaltyTransaction>> GetPagedAsync(int pageNumber, int pageSize, Guid? userId = null);
    }
}
