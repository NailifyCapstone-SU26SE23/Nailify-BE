using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Infrastructure.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Infrastructure.Repository
{
    public class SalonOperatingHourRepository : GenericRepository<SalonOperatingHour>, ISalonOperatingHourRepository
    {
        public SalonOperatingHourRepository(NailifyDbContext context) : base(context)
        {
        }

        public async Task<int> DeleteBySalonIdAsync(Guid salonId)
        {
            // Xóa set-based trực tiếp trên DB, không phụ thuộc entity đang track
            // → không bị DbUpdateConcurrencyException khi dữ liệu đã bị xóa/thay đổi trước đó
            return await _dbSet
                .Where(x => x.SalonId == salonId)
                .ExecuteDeleteAsync();
        }
    }
}
