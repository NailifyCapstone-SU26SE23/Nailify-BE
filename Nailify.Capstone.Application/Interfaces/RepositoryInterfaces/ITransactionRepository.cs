using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface ITransactionRepository : IGenericRepository<Transaction>
    {
        Task<Transaction?> GetByOrderCodeAsync(string orderCode, bool trackChanges = false);
        Task<Transaction?> GetDetailByIdAsync(int id, bool trackChanges = false);
        Task<IEnumerable<Transaction>> GetByBookingIdAsync(Guid bookingId);
        Task<(IEnumerable<Transaction> Items, int TotalItems)> GetPagedDetailAsync(
            int pageNumber,
            int pageSize,
            DateTime? startDate = null,
            DateTime? endDate = null,
            TransactionStatus? status = null,
            Guid? salonId = null,
            Guid? customerId = null);
    }
}
