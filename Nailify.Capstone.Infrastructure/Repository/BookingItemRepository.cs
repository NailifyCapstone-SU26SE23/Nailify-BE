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
    public class BookingItemRepository : GenericRepository<BookingItem>, IBookingItemRepository
    {
        public BookingItemRepository(NailifyDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<BookingItem>> GetBookingItemsByBookingIdAsync(Guid bookingId)
              => await FindByCondition(x => x.BookingId == bookingId)
                       .Include(x => x.NailVariant)
                       .Include(x => x.Service)
                       .ToListAsync();
    }
}
