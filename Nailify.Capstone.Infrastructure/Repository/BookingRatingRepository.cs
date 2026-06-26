using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRatingRequestDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Infrastructure.DBContext;

namespace Nailify.Capstone.Infrastructure.Repository
{
    public class BookingRatingRepository : GenericRepository<BookingRating>, IBookingRatingRepository
    {
        public BookingRatingRepository(NailifyDbContext context) : base(context)
        {
        }

        public Task<PagedList<BookingRating>> GetPagedAsync(BookingRatingRequestParameters parameters)
            => ToPagedListAsync(ApplyFilters(BuildQuery(), parameters), parameters.PageNumber, parameters.PageSize);

        public Task<BookingRating?> GetDetailByIdAsync(Guid id, bool trackChanges = false)
            => BuildQuery(trackChanges).FirstOrDefaultAsync(x => x.BookingRatingId == id);

        public Task<BookingRating?> GetByBookingIdAsync(Guid bookingId, bool trackChanges = false, bool includeDeleted = false)
            => BuildQuery(trackChanges, includeDeleted).FirstOrDefaultAsync(x => x.BookingId == bookingId);

        public Task<PagedList<BookingRating>> GetByBookingIdAsync(Guid bookingId, BookingRatingRequestParameters parameters)
            => ToPagedListAsync(ApplyFilters(BuildQuery().Where(x => x.BookingId == bookingId), parameters), parameters.PageNumber, parameters.PageSize);

        public Task<PagedList<BookingRating>> GetBySalonIdAsync(Guid salonId, BookingRatingRequestParameters parameters)
            => ToPagedListAsync(ApplyFilters(BuildQuery().Where(x => x.Booking.SalonId == salonId), parameters), parameters.PageNumber, parameters.PageSize);

        public Task<PagedList<BookingRating>> GetByNailArtistIdAsync(Guid nailArtistId, BookingRatingRequestParameters parameters)
            => ToPagedListAsync(ApplyFilters(BuildQuery().Where(x => x.Booking.NailArtistId == nailArtistId), parameters), parameters.PageNumber, parameters.PageSize);

        public Task<PagedList<BookingRating>> GetByCustomerIdAsync(Guid customerId, BookingRatingRequestParameters parameters)
            => ToPagedListAsync(ApplyFilters(BuildQuery().Where(x => x.CustomerId == customerId), parameters), parameters.PageNumber, parameters.PageSize);

        private IQueryable<BookingRating> BuildQuery(bool trackChanges = false, bool includeDeleted = false)
        {
            var query = trackChanges ? _dbSet : _dbSet.AsNoTracking();
            query = query
                .Include(x => x.Booking)
                    .ThenInclude(x => x.Salon)
                .Include(x => x.Booking)
                    .ThenInclude(x => x.NailArtist)
                .Include(x => x.Customer)
                    .ThenInclude(x => x.User);

            return includeDeleted
                ? query
                : query.Where(x => x.Status == "Active" && x.DeletedAt == null);
        }

        private static IQueryable<BookingRating> ApplyFilters(IQueryable<BookingRating> query, BookingRatingRequestParameters parameters)
        {
            if (parameters.StartDate.HasValue)
            {
                query = query.Where(x => x.CreatedAt >= parameters.StartDate.Value);
            }

            if (parameters.EndDate.HasValue)
            {
                query = query.Where(x => x.CreatedAt <= parameters.EndDate.Value);
            }

            if (parameters.Stars.HasValue)
            {
                query = query.Where(x => x.OverallScore == parameters.Stars.Value);
            }

            return query;
        }

        private static async Task<PagedList<BookingRating>> ToPagedListAsync(IQueryable<BookingRating> query, int pageNumber, int pageSize)
        {
            var count = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedList<BookingRating>(items, count, pageNumber, pageSize);
        }
    }
}
