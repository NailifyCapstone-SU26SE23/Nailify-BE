using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface IBookingItemRepository : IGenericRepository<BookingItem>
    {
        Task<IEnumerable<BookingItem>> GetBookingItemsByBookingIdAsync(Guid bookingId);
        Task<List<BookingItem>> GetCompletedBookingItemsWithVariantAsync();
        Task<Dictionary<int, int>> GetBookingCountsByVariantAsync();
        Task<HashSet<int>> GetBookedVariantIdsByCustomerIdAsync(Guid customerId);
    }
}
