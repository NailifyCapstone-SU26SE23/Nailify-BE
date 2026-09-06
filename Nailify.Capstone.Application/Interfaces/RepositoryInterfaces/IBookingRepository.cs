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
        Task<List<Booking>> GetApprovedBookingsWithDetailsByArtistAndDateAsync(Guid artistId, DateTime date);
        /// <summary>
        /// Lấy danh sách booking đang chiếm ghế tại thời điểm chỉ định (CheckedIn / InProgress),
        /// bao gồm thông tin Customer để hiển thị trên dashboard ghế.
        /// </summary>
        Task<IEnumerable<Booking>> GetChairOccupancyBySalonAsync(Guid salonId, DateTime date, TimeSpan atTime);
        /// <summary>
        /// Lấy đơn đặt lịch đang được thực hiện (InProgress) của một thợ, loại trừ một đơn cụ thể.
        /// Sử dụng để kiểm tra đè ca.
        /// </summary>
        Task<Booking?> GetCurrentBusyBookingWithProceduresAsync(Guid artistId, Guid excludeBookingId, DateTime todayDate);
        
        /// <summary>
        /// Tìm đơn bảo hành (warranty booking) của một đơn gốc.
        /// Trả về null nếu chưa có đơn bảo hành active.
        /// </summary>
        Task<Booking?> GetWarrantyBookingAsync(Guid originalBookingId);
        /// <summary>
        /// Đếm số booking có Status = Approved của một salon
        /// mà thời gian thực hiện bị overlap với booking chỉ định,
        /// loại trừ chính booking đó.
        /// </summary>
        Task<int> CountApprovedOverlappingAsync(Guid salonId, DateTime bookingDate, TimeSpan startTime, int durationMinutes, Guid? excludeBookingId = null);
        Task<List<Booking>> GetLateCancelledBookingsBySalonAsync(Guid salonId, DateTime date);

        /// <summary>
        /// Lấy danh sách các ca đặt lịch đang thực hiện (InProgress) trong ngày nhưng đã quá giờ kết thúc dự kiến.
        /// </summary>
        Task<IEnumerable<Booking>> GetOverdueInProgressBookingsAsync(DateTime date, TimeSpan currentTime, bool trackChanges = false);
        /// <summary>
        /// Lấy đơn đặt lịch tiếp theo (Approved/Pending) của một thợ sau một mốc thời gian cụ thể.
        /// </summary>
        Task<Booking?> GetNextBookingForArtistAsync(Guid artistId, DateTime date, TimeSpan afterTime, bool trackChanges = false);
    }
}
