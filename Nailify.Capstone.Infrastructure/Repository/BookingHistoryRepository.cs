using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Infrastructure.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Infrastructure.Repository
{
    public class BookingHistoryRepository : GenericRepository<BookingHistory>, IBookingHistoryRepository
    {
        public BookingHistoryRepository(NailifyDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<BookingHistory>> GetBookingHistoriesByBookingIdAsync(Guid bookingId)
        {
            return await FindByCondition(bh => bh.BookingId == bookingId)
                         .Include(bh => bh.Actor)
                         .OrderBy(bh => bh.CreatedAt)
                         .ToListAsync();
        }
    }
}
