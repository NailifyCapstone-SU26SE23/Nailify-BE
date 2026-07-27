using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface IScheduleRepository : IGenericRepository<Schedule>
    {
        Task<IEnumerable<Schedule>> GetSchedulesByArtistIdAsync(Guid artistId, DateTime? startDate, DateTime? endDate);
        Task<IEnumerable<Schedule>> GetSchedulesBySalonIdAsync(Guid salonId, DateTime? startDate, DateTime? endDate);
        Task<Schedule?> GetScheduleByArtistAndDateAsync(Guid artistId, DateTime date);
        /// <summary>
        /// Đếm số lượng thợ móng thực tế có ca làm việc hoạt động tại Salon vào một Ngày cụ thể.
        /// </summary>
        Task<int> GetWorkingArtistCountByDateAsync(Guid salonId, DateTime date);
    }
}
