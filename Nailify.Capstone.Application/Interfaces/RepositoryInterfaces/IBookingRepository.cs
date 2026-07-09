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
        Task<Booking?> GetBookingDetailAsync(Guid bookingId, bool trackChanges = false);
        Task<IEnumerable<Booking>> GetBookingsByArtistAndDateAsync(Guid artistId, DateTime date);
        Task<IEnumerable<Booking>> GetBookingsByChairAndDateAsync(Guid chairId, DateTime date);
        Task<IEnumerable<Booking>> GetActiveBookingsWithChairsBySalonAndDateAsync(Guid salonId, DateTime date);
        Task<PagedList<Booking>> GetBookingsByCustomerAsync(Guid customerId, int pageNumber, int pageSize, DateTime? startDate = null, DateTime? endDate = null, BookingStatus? status = null);
        Task<PagedList<Booking>> GetBookingsBySalonAsync(Guid salonId, int pageNumber, int pageSize, DateTime? startDate = null, DateTime? endDate = null, BookingStatus? status = null, string? search = null);
        Task<PagedList<Booking>> GetBookingsByArtistAsync(Guid artistId, int pageNumber, int pageSize, DateTime? startDate = null, DateTime? endDate = null, BookingStatus? status = null, string? search = null);
        /// <summary>
        /// Lấy danh sách các đơn đặt lịch có trạng thái Approved nhưng đã quá giờ check-in so với mốc thời gian chỉ định.
        /// </summary>
        /// <param name="date">Ngày cần kiểm tra (ví dụ: ngày hôm nay)</param>
        /// <param name="thresholdTime">Mốc giờ giới hạn (ví dụ: giờ hiện tại trừ đi 15 phút)</param>
        /// <param name="trackChanges">Có theo dõi thay đổi thực thể hay không</param>
        /// <returns>Danh sách các Booking đã quá hạn check-in</returns>
        Task<IEnumerable<Booking>> GetOverdueApprovedBookingsAsync(DateTime date, TimeSpan thresholdTime, bool trackChanges = false);
        Task<int> CountServingBookingsAsync(Guid artistId, DateTime date);
        Task<int> CountUpcomingBookingsAsync(Guid artistId, DateTime date, TimeSpan startTime, TimeSpan thresholdTime);
        Task<List<Booking>> GetCompletedBookingsWithDetailsAsync(Guid customerId);
    }
}
