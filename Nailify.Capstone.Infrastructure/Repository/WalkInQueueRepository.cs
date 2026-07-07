using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;
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

        public async Task<int> GetNextPositionAsync(Guid salonId, Guid? assignedNailArtistId)
        {
            var today = DateTime.UtcNow.Date;
            var maxPos = await FindByCondition(x => x.SalonId == salonId 
                                               && x.ArrivalTime.Date == today
                                               && x.AssignedNailArtistId == assignedNailArtistId)
                            .MaxAsync(x => (int?)x.QueuePosition) ?? 0;
            return maxPos + 1;
        }

        public async Task<int> GetNextPositionAsync(Guid salonId)
        {
            var today = DateTime.UtcNow.Date;
            var maxPos = await FindByCondition(x => x.SalonId == salonId 
                                               && x.ArrivalTime.Date == today)
                            .MaxAsync(x => (int?)x.QueuePosition) ?? 0;
            return maxPos + 1;
        }

        public async Task<IEnumerable<WalkInQueue>> GetTodayQueueAsync(Guid salonId, bool trackChanges = false)
        {
            var today = DateTime.UtcNow.Date;
            return await FindByCondition(x => x.SalonId == salonId 
                                         && x.ArrivalTime.Date == today,
                                         trackChanges)
                         .Include(x => x.Customer)
                         .Include(x => x.AssignedNailArtist)
                         .ToListAsync();
        }
        public async Task<IEnumerable<WalkInQueue>> GetActiveWaitingEntriesAsync(Guid salonId, Guid? assignedNailArtistId,bool trackChanges = false)
        {
            var today = DateTime.UtcNow.Date;
            return await FindByCondition(x => x.SalonId == salonId
                                         && x.Status == QueueStatus.Waiting
                                         && x.ArrivalTime.Date == today
                                          && x.AssignedNailArtistId == assignedNailArtistId, 
trackChanges)
                         .ToListAsync();
        }

        public async Task<int> CountServingWalkInsAsync(Guid artistId, DateTime date)
        {
            var today = date.Date;
            return await FindByCondition(x => x.AssignedNailArtistId == artistId
                                           && x.ArrivalTime.Date == today
                                           && x.Status == QueueStatus.InService)
                        .CountAsync();
        }

    }
}
