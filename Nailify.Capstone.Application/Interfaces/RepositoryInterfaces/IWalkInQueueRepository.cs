using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface IWalkInQueueRepository : IGenericRepository<WalkInQueue>
    {
        /// <summary>
        /// Trả về danh sách các khách hàng trong hàng đợi hôm nay của salon dựa trên salonId.
        /// </summary>
        /// <param name="salonId">ID của salon</param>
        /// <returns>Danh sách các khách hàng trong hàng đợi hôm nay</returns>
        Task<IEnumerable<WalkInQueue>> GetTodayQueueAsync(Guid salonId, bool trackChanges = false);
        /// <summary>
        /// Lấy vị trí tiếp theo trong hàng đợi cho một salon cụ thể.
        /// </summary>
        /// <param name="salonId">ID của salon</param>
        /// <returns>Vị trí tiếp theo trong hàng đợi</returns>
        Task<int> GetNextPositionAsync(Guid salonId);
        /// <summary>
        /// Lấy danh sách khách hàng đang chờ (Waiting) tại sảnh hôm nay của Salon.
        /// </summary>
        Task<IEnumerable<WalkInQueue>> GetActiveWaitingEntriesAsync(Guid salonId, Guid? assignedNailArtistId, bool trackChanges = false);
        // Lấy số thứ tự tiếp theo của thợ đó tại salon trong ngày hôm nay
        Task<int> GetNextPositionAsync(Guid salonId, Guid? assignedNailArtistId);
        Task<int> CountServingWalkInsAsync(Guid artistId, DateTime date);
    }
}
