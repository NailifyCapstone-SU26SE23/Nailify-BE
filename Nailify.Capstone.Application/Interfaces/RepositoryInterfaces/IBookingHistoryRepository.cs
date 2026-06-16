using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface IBookingHistoryRepository : IGenericRepository<BookingHistory>
    {
        Task<PagedList<BookingHistory>> GetPagedBookingHistoriesAsync(int pageNumber, int pageSize, DateTime? startDate = null, DateTime? endDate = null);
        Task<BookingHistory?> GetBookingHistoryDetailAsync(Guid bookingHistoryId);
        Task<PagedList<BookingHistory>> GetPagedBookingHistoriesByBookingIdAsync(Guid bookingId, int pageNumber, int pageSize, DateTime? startDate = null, DateTime? endDate = null);
        Task<PagedList<BookingHistory>> GetPagedBookingHistoriesBySalonIdAsync(Guid salonId, int pageNumber, int pageSize, DateTime? startDate = null, DateTime? endDate = null);
        Task<PagedList<BookingHistory>> GetPagedBookingHistoriesByArtistIdAsync(Guid artistId, int pageNumber, int pageSize, DateTime? startDate = null, DateTime? endDate = null);
        Task<IEnumerable<BookingHistory>> GetBookingHistoriesByBookingIdAsync(Guid bookingId);
    }
}
