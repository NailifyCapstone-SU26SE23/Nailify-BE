using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface ISalonOperatingHourRepository : IGenericRepository<SalonOperatingHour>
    {
        /// <summary>
        /// Xóa toàn bộ giờ hoạt động của một Salon trực tiếp trên DB (set-based).
        /// </summary>
        Task<int> DeleteBySalonIdAsync(Guid salonId);
    }
}
