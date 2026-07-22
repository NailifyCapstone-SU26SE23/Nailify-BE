using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Infrastructure.DBContext;

namespace Nailify.Capstone.Infrastructure.Repository
{
    public class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(NailifyDbContext context) : base(context)
        {
        }

        public async Task<Transaction?> GetByOrderCodeAsync(string orderCode, bool trackChanges = false)
        {
            return await FindByCondition(t => t.OrderCode == orderCode, trackChanges)
                .Include(t => t.Booking)
                .FirstOrDefaultAsync();
        }

        public async Task<Guid?> GetBookingIdByOrderCodeAsync(string orderCode)
        {
            return await FindByCondition(t => t.OrderCode == orderCode, false)
                .Select(t => (Guid?)t.BookingId)
                .FirstOrDefaultAsync();
        }

        public async Task<Transaction?> GetDetailByIdAsync(int id, bool trackChanges = false)
        {
            return await BuildDetailQuery(trackChanges)
                .FirstOrDefaultAsync(t => t.TransactionId == id);
        }

        public async Task<IEnumerable<Transaction>> GetByBookingIdAsync(Guid bookingId)
        {
            return await BuildDetailQuery()
                .Where(t => t.BookingId == bookingId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Transaction> Items, int TotalItems)> GetPagedDetailAsync(
            int pageNumber,
            int pageSize,
            DateTime? startDate = null,
            DateTime? endDate = null,
            TransactionStatus? status = null,
            Guid? salonId = null,
            Guid? customerId = null)
        {
            var query = BuildDetailQuery();

            if (startDate.HasValue) query = query.Where(t => t.CreatedAt >= startDate.Value);
            if (endDate.HasValue) query = query.Where(t => t.CreatedAt <= endDate.Value);
            if (status.HasValue) query = query.Where(t => t.Status == status.Value);
            if (salonId.HasValue) query = query.Where(t => t.Booking.SalonId == salonId.Value);
            if (customerId.HasValue) query = query.Where(t => t.Booking.CustomerId == customerId.Value);

            var totalItems = await query.CountAsync();
            var items = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalItems);
        }

        private IQueryable<Transaction> BuildDetailQuery(bool trackChanges = false)
        {
            var query = trackChanges ? _context.Transactions : _context.Transactions.AsNoTracking();
            return query
                .Include(t => t.Booking)
                .ThenInclude(b => b.Customer)
                .ThenInclude(c => c.User)
                .Include(t => t.Booking)
                .ThenInclude(b => b.Salon);
        }
    }
}
