using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;
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

        public async Task<Booking?> GetBookingDetailAsync(Guid bookingId, bool trackChanges = false)
           => await FindByCondition(x => x.BookingId == bookingId, trackChanges)
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
                                    .Include(x => x.BookingDiscounts)
                                    .Include(x => x.BookingHistories)
                                    .FirstOrDefaultAsync();

        public async Task<IEnumerable<Booking>> GetBookingsByArtistAndDateAsync(Guid artistId, DateTime date)
        {
            var range = GetDateRangeUtc(date);
            return await FindByCondition(x =>
                                        x.NailArtistId == artistId
                                        && x.BookingDate >= range.start
                                        && x.BookingDate <= range.end
                                        && x.Status != BookingStatus.Cancelled 
                                        && x.Status != BookingStatus.Rejected)
                                    .ToListAsync();
        }

        public async Task<PagedList<Booking>> GetBookingsByCustomerAsync(Guid customerId, int pageNumber, int pageSize, DateTime? startDate = null, DateTime? endDate = null, BookingStatus? status = null)
        {
            var query = BuildBookingQuery()
                .Where(x => x.CustomerId == customerId);

            query = ApplyBookingFilters(query, startDate, endDate, status);

            return await ToPagedListAsync(query, pageNumber, pageSize);
        }

        public async Task<PagedList<Booking>> GetBookingsBySalonAsync(Guid salonId, int pageNumber, int pageSize, DateTime? startDate = null, DateTime? endDate = null, BookingStatus? status = null, string? search = null)
        {
            var query = BuildBookingQuery()
                .Where(b => b.SalonId == salonId);

            query = ApplyBookingFilters(query, startDate, endDate, status);
            query = ApplyCustomerSearch(query, search);

            return await ToPagedListAsync(query, pageNumber, pageSize);
        }

        public async Task<bool> HasBookingConflictAsync(Guid artistId, DateTime date, TimeSpan startTime, TimeSpan endTime)
        {
            var bookings = await GetBookingsByArtistAndDateAsync(artistId, date);
            return bookings.Any(x => x.StartTime < endTime && x.StartTime.Add(TimeSpan.FromMinutes(x.TotalDuration)) > startTime);
        }

        public async Task<bool> HasBookingConflictExcludingCurrentAsync(Guid artistId, DateTime date, TimeSpan startTime, TimeSpan endTime, Guid currentBookingId)
        {
            var bookings = await GetBookingsByArtistAndDateAsync(artistId, date);
            return bookings.Any(x => x.BookingId != currentBookingId && x.StartTime < endTime && x.StartTime.Add(TimeSpan.FromMinutes(x.TotalDuration)) > startTime);
        }

        public async Task<PagedList<Booking>> GetBookingsByArtistAsync(Guid artistId, int pageNumber, int pageSize, DateTime? startDate = null, DateTime? endDate = null, BookingStatus? status = null, string? search = null)
        {
            var query = BuildBookingQuery()
                .Where(x => x.NailArtistId == artistId);

            query = ApplyBookingFilters(query, startDate, endDate, status);
            query = ApplyCustomerSearch(query, search);

            return await ToPagedListAsync(query, pageNumber, pageSize);
        }

        private IQueryable<Booking> BuildBookingQuery()
        {
            return _dbSet
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
                .Include(x => x.BookingDiscounts);
        }

        private IQueryable<Booking> ApplyBookingFilters(IQueryable<Booking> query, DateTime? startDate, DateTime? endDate, BookingStatus? status)
        {
            if (startDate.HasValue)
            {
                var range = GetDateRangeUtc(startDate.Value);
                query = query.Where(x => x.BookingDate >= range.start);
            }

            if (endDate.HasValue)
            {
                var range = GetDateRangeUtc(endDate.Value);
                query = query.Where(x => x.BookingDate <= range.end);
            }

            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status.Value);
            }

            return query;
        }

        private static IQueryable<Booking> ApplyCustomerSearch(IQueryable<Booking> query, string? search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                return query;
            }

            var keyword = search.Trim().ToLower();
            return query.Where(x =>
                (x.Customer.User.FirstName + " " + x.Customer.User.LastName).ToLower().Contains(keyword)
                || (x.Customer.User.Phone != null && x.Customer.User.Phone.Contains(keyword)));
        }

        private static async Task<PagedList<Booking>> ToPagedListAsync(IQueryable<Booking> query, int pageNumber, int pageSize)
        {
            var count = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.BookingDate)
                .ThenByDescending(x => x.StartTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedList<Booking>(items, count, pageNumber, pageSize);
        }
    }
}
