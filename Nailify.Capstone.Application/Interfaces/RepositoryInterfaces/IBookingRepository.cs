using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface IBookingRepository : IGenericRepository<Booking>
    {
        Task<Booking?> GetBookingDetailAsync(Guid bookingId);
        Task<IEnumerable<Booking>> GetBookingsByArtistAndDateAsync(Guid artistId, DateTime date);
        Task<PagedList<Booking>> GetBookingsByCustomerAsync(Guid customerId, int pageNumber, int pageSize, DateTime? startDate = null, DateTime? endDate = null, BookingStatus? status = null);
        Task<PagedList<Booking>> GetBookingsBySalonAsync(Guid salonId, int pageNumber, int pageSize, DateTime? startDate = null, DateTime? endDate = null, BookingStatus? status = null, string? search = null);
        Task<bool> HasBookingConflictAsync(Guid artistId, DateTime date, TimeSpan startTime, TimeSpan endTime);
        Task<bool> HasBookingConflictExcludingCurrentAsync(Guid artistId, DateTime date, TimeSpan startTime, TimeSpan endTime, Guid currentBookingId);
        Task<PagedList<Booking>> GetBookingsByArtistAsync(Guid artistId, int pageNumber, int pageSize, DateTime? startDate = null, DateTime? endDate = null, BookingStatus? status = null, string? search = null);
    }
}
