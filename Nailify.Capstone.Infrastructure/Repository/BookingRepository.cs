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
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        public BookingRepository(NailifyDbContext context) : base(context)
        {
        }

        private (DateTime start, DateTime end) GetDateRangeUtc(DateTime date)
        {
            var localDate = (date.Kind == DateTimeKind.Utc ? date.AddHours(7) : date).Date;
            var start = DateTime.SpecifyKind(localDate.AddHours(-7), DateTimeKind.Utc);
            var end = start.AddDays(1).AddTicks(-1);
            return (start, end);
        }

        public async Task<Booking?> GetBookingDetailAsync(Guid bookingId)
           => await FindByCondition(x => x.BookingId == bookingId)
                                    .Include(x => x.Customer)
                                       .ThenInclude(x => x.User)
                                    .Include(x => x.Salon)
                                    .Include(x => x.NailArtist)
                                       .ThenInclude(x => x.Account)
                                    .Include(x => x.BookingItems)
                                       .ThenInclude(x => x.NailVariant)
                                    .Include(x => x.BookingItems)
                                       .ThenInclude(x => x.Service)
                                    .Include(x => x.BookingItems)
                                       .ThenInclude(x => x.CustomerNail)
                                    .Include(x => x.BookingHistories)
                                    .FirstOrDefaultAsync();

        public async Task<IEnumerable<Booking>> GetBookingsByArtistAndDateAsync(Guid artistId, DateTime date)
        {
            var range = GetDateRangeUtc(date);
            return await FindByCondition(x =>
                                        x.NailArtistId == artistId
                                        && x.BookingDate >= range.start
                                        && x.BookingDate <= range.end
                                        && x.Status != "Cancelled" 
                                        && x.Status != "Rejected")
                                    .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByCustomerAsync(Guid customerId)
            => await FindByCondition(x => x.CustomerId == customerId)
                                    .Include(x => x.Salon)
                                    .Include(x => x.NailArtist)
                                        .ThenInclude(x => x.Account)
                                    .OrderByDescending(x => x.BookingDate)
                                    .ThenByDescending(x => x.ExpectedTime)
                                    .ToListAsync();

        public async Task<IEnumerable<Booking>> GetBookingsBySalonAsync(Guid salonId)
        {
            return await FindByCondition(b => b.SalonId == salonId)
                         .Include(b => b.Customer)
                            .ThenInclude(c => c.User)
                         .Include(b => b.NailArtist)
                            .ThenInclude(na => na.Account)
                          .OrderByDescending(b => b.BookingDate)
                          .ToListAsync();
        }

        public async Task<bool> HasBookingConflictAsync(Guid artistId, DateTime date, TimeSpan startTime, TimeSpan endTime)
        {
            var range = GetDateRangeUtc(date);
            return await ExistsAsync(x =>
                                         x.NailArtistId == artistId &&
                                         x.BookingDate >= range.start &&
                                         x.BookingDate <= range.end &&
                                         x.Status != "Cancelled" &&
                                         x.Status != "Rejected" &&
                                        (
                                            (x.ExpectedTime < endTime && x.ExpectedTime.Add(TimeSpan.FromMinutes(x.TotalDuration)) > startTime)
                                        )
                                    );
        }

        public async Task<bool> HasBookingConflictExcludingCurrentAsync(Guid artistId, DateTime date, TimeSpan startTime, TimeSpan endTime, Guid currentBookingId)
        {
            var range = GetDateRangeUtc(date);
            return await ExistsAsync(x =>
                                         x.BookingId != currentBookingId &&
                                         x.NailArtistId == artistId &&
                                         x.BookingDate >= range.start &&
                                         x.BookingDate <= range.end &&
                                         x.Status != "Cancelled" &&
                                         x.Status != "Rejected" &&
                                        (
                                            (x.ExpectedTime < endTime && x.ExpectedTime.Add(TimeSpan.FromMinutes(x.TotalDuration)) > startTime)
                                        )
                                    );
        }
    }
}
