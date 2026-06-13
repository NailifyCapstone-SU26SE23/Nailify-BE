using Nailify.Capstone.Domain.Entities;
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
        Task<IEnumerable<Booking>> GetBookingsByCustomerAsync(Guid customerId);
        Task<IEnumerable<Booking>> GetBookingsBySalonAsync(Guid salonId);
        Task<bool> HasBookingConflictAsync(Guid artistId, DateTime date, TimeSpan startTime, TimeSpan endTime);
        Task<bool> HasBookingConflictExcludingCurrentAsync(Guid artistId, DateTime date, TimeSpan startTime, TimeSpan endTime, Guid currentBookingId);
    }
}
