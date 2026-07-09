using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRatingRequestDTOs;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface IBookingRatingRepository : IGenericRepository<BookingRating>
    {
        Task<PagedList<BookingRating>> GetPagedAsync(BookingRatingRequestParameters parameters);
        Task<BookingRating?> GetDetailByIdAsync(Guid id, bool trackChanges = false);
        Task<BookingRating?> GetByBookingIdAsync(Guid bookingId, bool trackChanges = false, bool includeDeleted = false);
        Task<PagedList<BookingRating>> GetByBookingIdAsync(Guid bookingId, BookingRatingRequestParameters parameters);
        Task<PagedList<BookingRating>> GetBySalonIdAsync(Guid salonId, BookingRatingRequestParameters parameters);
        Task<PagedList<BookingRating>> GetByNailArtistIdAsync(Guid nailArtistId, BookingRatingRequestParameters parameters);
        Task<PagedList<BookingRating>> GetByCustomerIdAsync(Guid customerId, BookingRatingRequestParameters parameters);
        Task<List<BookingRating>> GetAllWithBookingItemsAsync();
    }
}
