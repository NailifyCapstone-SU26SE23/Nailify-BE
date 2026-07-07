using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface IBookingDiscountRepository : IGenericRepository<BookingDiscount>
    {
        Task<List<BookingDiscount>> GetByBookingIdAsync(Guid bookingId);
    }
}
