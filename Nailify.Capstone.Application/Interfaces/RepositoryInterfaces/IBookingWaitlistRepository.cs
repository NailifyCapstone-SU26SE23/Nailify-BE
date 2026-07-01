using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface IBookingWaitlistRepository : IGenericRepository<BookingWaitlist>
    {
        /// <summary>
        /// Lấy người tiếp theo trong danh sách chờ cho một salon cụ thể vào một ngày và giờ bắt đầu nhất định.
        /// </summary>
        /// <param name="salonId">ID của salon</param>
        /// <param name="date">Ngày đặt lịch</param>
        /// <param name="startTime">Giờ bắt đầu</param>
        /// <returns>Người tiếp theo trong danh sách chờ hoặc null nếu không có</returns>
        Task<BookingWaitlist?> GetNextWaitingEntryAsync(Guid salonId, DateTime date, TimeSpan startTime);
        /// <summary>
        /// Kiểm tra xem khách hàng đã có trong danh sách chờ cho một salon cụ thể vào một ngày và giờ bắt đầu nhất định hay chưa.
        /// </summary>
        /// <param name="customerId">ID của khách hàng</param>
        /// <param name="salonId">ID của salon</param>
        /// <param name="date">Ngày đặt lịch</param>
        /// <param name="startTime">Giờ bắt đầu</param>
        /// <returns>True nếu khách hàng đã có trong danh sách chờ, ngược lại False</returns>
        Task<bool> IsDuplicateAsync(Guid customerId, Guid salonId, DateTime date, TimeSpan startTime);
        /// <summary>
        /// Lấy vị trí tiếp theo trong danh sách chờ cho một salon cụ thể vào một ngày và giờ bắt đầu nhất định.
        /// </summary>
        /// <param name="salonId">ID của salon</param>
        /// <param name="date">Ngày đặt lịch</param>
        /// <param name="startTime">Giờ bắt đầu</param>
        /// <returns>Vị trí tiếp theo trong danh sách chờ</returns>
        Task<int> GetNextPositionAsync(Guid salonId, DateTime date, TimeSpan startTime);
        /// <summary>
        /// Lấy danh sách các lượt trong hàng chờ có trạng thái Notified và đã quá thời gian hết hạn.
        /// </summary>
        /// <param name="trackChanges">Có theo dõi thay đổi thực thể hay không</param>
        /// <returns>Danh sách các BookingWaitlist đã hết hạn</returns>
        Task<IEnumerable<BookingWaitlist>> GetExpiredOrPastEntriesAsync(DateTime referenceDateTime, bool trackChanges = false);

    }
}
