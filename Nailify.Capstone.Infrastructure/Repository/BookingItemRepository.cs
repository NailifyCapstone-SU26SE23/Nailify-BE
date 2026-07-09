using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Infrastructure.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nailify.Capstone.Domain.Enums;
namespace Nailify.Capstone.Infrastructure.Repository
{
    public class BookingItemRepository : GenericRepository<BookingItem>, IBookingItemRepository
    {
        public BookingItemRepository(NailifyDbContext context) : base(context)
        {
        }

        public async Task<HashSet<int>> GetBookedVariantIdsByCustomerIdAsync(Guid customerId)
        {
            var ids = await FindByCondition(bi => bi.Booking.CustomerId == customerId && bi.NailVariantId != null)
                 .Select(bi => bi.NailVariantId!.Value)
                 .Distinct()
                 .ToListAsync();

            return ids.ToHashSet();
        }

        public async Task<Dictionary<int, int>> GetBookingCountsByVariantAsync()
        {
            return await FindByCondition(bi => bi.NailVariantId != null &&
                                          (bi.Booking.Status == BookingStatus.Completed ||
                                           bi.Booking.Status == BookingStatus.ServiceCompleted))
                  .GroupBy(bi => bi.NailVariantId!.Value)
                  .Select(g => new { NailVariantId = g.Key, Count = g.Count() })
                  .ToDictionaryAsync(x => x.NailVariantId, x => x.Count);
        }

        public async Task<IEnumerable<BookingItem>> GetBookingItemsByBookingIdAsync(Guid bookingId)
              => await FindByCondition(x => x.BookingId == bookingId)
                       .Include(x => x.NailVariant)
                       .Include(x => x.Service)
                       .ToListAsync();

        public Task<List<BookingItem>> GetCompletedBookingItemsWithVariantAsync()
          => FindByCondition(x => x.NailVariantId != null 
                             && (x.Booking.Status == BookingStatus.Completed 
                                 || x.Booking.Status == BookingStatus.ServiceCompleted))
            .Include(x => x.Booking)
            .ToListAsync();
    }
}
