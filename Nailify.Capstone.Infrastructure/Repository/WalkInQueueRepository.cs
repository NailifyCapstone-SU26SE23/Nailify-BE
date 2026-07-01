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
    public class WalkInQueueRepository : GenericRepository<WalkInQueue>, IWalkInQueueRepository
    {
        public WalkInQueueRepository(NailifyDbContext context) : base(context)
        {
        }

        public async Task<int> GetNextPositionAsync(Guid salonId)
        {
            var today = DateTime.UtcNow.Date;
            var maxPos = await FindByCondition(x => x.SalonId == salonId 
                                               && x.ArrivalTime.Date == today)
                            .MaxAsync(x => (int?)x.QueuePosition) ?? 0;
            return maxPos + 1;
        }

        public async Task<IEnumerable<WalkInQueue>> GetTodayQueueAsync(Guid salonId)
        {
            var today = DateTime.UtcNow.Date;
            return await FindByCondition(x => x.SalonId == salonId 
                                         && x.ArrivalTime.Date == today)
                         .Include(x => x.Customer)
                         .Include(x => x.AssignedNailArtist)
                         .Include(x => x.QueuePosition)
                         .ToListAsync();
        }
    }
}
