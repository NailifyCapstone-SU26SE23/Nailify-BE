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
        Task<IEnumerable<WalkInQueue>> GetTodayQueueAsync(Guid salonId);
        /// <summary>
        /// Lấy vị trí tiếp theo trong hàng đợi cho một salon cụ thể.
        /// </summary>
        /// <param name="salonId">ID của salon</param>
        /// <returns>Vị trí tiếp theo trong hàng đợi</returns>
        Task<int> GetNextPositionAsync(Guid salonId);
    }
}
