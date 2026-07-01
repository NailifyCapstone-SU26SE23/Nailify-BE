using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Infrastructure.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nailify.Capstone.Domain.Enums;
using Microsoft.EntityFrameworkCore;
namespace Nailify.Capstone.Infrastructure.Repository
{
    public class BookingWaitlistRepository : GenericRepository<BookingWaitlist>, IBookingWaitlistRepository
    {
        public BookingWaitlistRepository(NailifyDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<BookingWaitlist>> GetExpiredOrPastEntriesAsync(DateTime referenceDateTime, bool trackChanges = false)
         => await FindByCondition(x => x.Status == WaitlistStatus.Notified
                             && x.ExpiresAt.HasValue
                             && x.ExpiresAt.Value < referenceDateTime, trackChanges)
                  .ToListAsync();


        public async Task<int> GetNextPositionAsync(Guid salonId, DateTime date, TimeSpan startTime)
        {
            // Read-only count, using default AsNoTracking via FindByCondition
            var maxPosition = await FindByCondition(x => x.SalonId == salonId 
                                                    && x.RequestedDate.Date == date.Date
                                                    && x.RequestedStartTime == startTime)
                            .MaxAsync(x => (int?)x.Position) ?? 0;
            return maxPosition + 1;
        }

        public async Task<BookingWaitlist?> GetNextWaitingEntryAsync(Guid salonId, DateTime date, TimeSpan startTime)
          => await FindByCondition(x => x.SalonId == salonId 
                                   && x.RequestedDate.Date == date.Date
                                   && x.RequestedStartTime == startTime
                                   && x.Status == WaitlistStatus.Waiting,true)
                   .OrderBy(x => x.Position)
                   .FirstOrDefaultAsync();   

        public async Task<bool> IsDuplicateAsync(Guid customerId, Guid salonId, DateTime date, TimeSpan startTime)
        {
            // Read-only check, using default AsNoTracking via FindByCondition
            return await FindByCondition(x => x.CustomerId == customerId 
                                         && x.SalonId == salonId 
                                         && x.RequestedDate.Date == date.Date
                                         && x.RequestedStartTime == startTime
                                        && x.Status == WaitlistStatus.Waiting)
                        .AnyAsync();
        }
    }
}
