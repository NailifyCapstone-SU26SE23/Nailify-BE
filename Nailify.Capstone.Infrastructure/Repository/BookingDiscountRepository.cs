using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Infrastructure.DBContext;

namespace Nailify.Capstone.Infrastructure.Repository
{
    public class BookingDiscountRepository : GenericRepository<BookingDiscount>, IBookingDiscountRepository
    {
        public BookingDiscountRepository(NailifyDbContext context) : base(context)
        {
        }

        public async Task<List<BookingDiscount>> GetByBookingIdAsync(Guid bookingId)
        {
            return await _dbSet
                .Where(discount => discount.BookingId == bookingId)
                .OrderBy(discount => discount.AppliedDate)
                .ToListAsync();
        }
    }
}
