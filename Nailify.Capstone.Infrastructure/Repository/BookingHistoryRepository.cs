using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Infrastructure.DBContext;

namespace Nailify.Capstone.Infrastructure.Repository
{
    public class BookingHistoryRepository : GenericRepository<BookingHistory>, IBookingHistoryRepository
    {
        public BookingHistoryRepository(NailifyDbContext context) : base(context)
        {
        }

        public async Task<PagedList<BookingHistory>> GetPagedBookingHistoriesAsync(int pageNumber, int pageSize, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = ApplyDateFilter(BuildBookingHistoryQuery(), startDate, endDate);

            var count = await query.CountAsync();
            var items = await query
                .OrderByDescending(history => history.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedList<BookingHistory>(items, count, pageNumber, pageSize);
        }

        public async Task<BookingHistory?> GetBookingHistoryDetailAsync(Guid bookingHistoryId)
        {
            return await BuildBookingHistoryQuery()
                .FirstOrDefaultAsync(history => history.BookingHistoryId == bookingHistoryId);
        }

        public async Task<PagedList<BookingHistory>> GetPagedBookingHistoriesByBookingIdAsync(Guid bookingId, int pageNumber, int pageSize, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = BuildBookingHistoryQuery()
                .Where(history => history.BookingId == bookingId);
            query = ApplyDateFilter(query, startDate, endDate);

            return await ToPagedListAsync(query, pageNumber, pageSize);
        }

        public async Task<PagedList<BookingHistory>> GetPagedBookingHistoriesBySalonIdAsync(Guid salonId, int pageNumber, int pageSize, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = BuildBookingHistoryQuery()
                .Where(history => history.Booking.SalonId == salonId);
            query = ApplyDateFilter(query, startDate, endDate);

            return await ToPagedListAsync(query, pageNumber, pageSize);
        }

        public async Task<PagedList<BookingHistory>> GetPagedBookingHistoriesByArtistIdAsync(Guid artistId, int pageNumber, int pageSize, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = BuildBookingHistoryQuery()
                .Where(history => history.Booking.NailArtistId == artistId);
            query = ApplyDateFilter(query, startDate, endDate);

            return await ToPagedListAsync(query, pageNumber, pageSize);
        }

        public async Task<IEnumerable<BookingHistory>> GetBookingHistoriesByBookingIdAsync(Guid bookingId)
        {
            return await BuildBookingHistoryQuery()
                .Where(history => history.BookingId == bookingId)
                .OrderBy(history => history.CreatedAt)
                .ToListAsync();
        }

        private IQueryable<BookingHistory> BuildBookingHistoryQuery()
        {
            return _dbSet
                .Include(history => history.Actor)
                .Include(history => history.Booking);
        }

        private static IQueryable<BookingHistory> ApplyDateFilter(IQueryable<BookingHistory> query, DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue)
            {
                query = query.Where(history => history.CreatedAt >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                var exclusiveEndDate = endDate.Value.Date.AddDays(1);
                query = query.Where(history => history.CreatedAt < exclusiveEndDate);
            }

            return query;
        }

        private static async Task<PagedList<BookingHistory>> ToPagedListAsync(IQueryable<BookingHistory> query, int pageNumber, int pageSize)
        {
            var count = await query.CountAsync();
            var items = await query
                .OrderByDescending(history => history.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedList<BookingHistory>(items, count, pageNumber, pageSize);
        }
    }
}
