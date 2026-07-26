using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Infrastructure.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nailify.Capstone.Infrastructure.Repository
{
    public class ScheduleRepository : GenericRepository<Schedule>, IScheduleRepository
    {
        public ScheduleRepository(NailifyDbContext context) : base(context) { }

        public async Task<Schedule?> GetScheduleByArtistAndDateAsync(Guid artistId, DateTime date)
        {
            var localDate = (date.Kind == DateTimeKind.Utc ? date.AddHours(7) : date).Date;
            var startOfDayUtc = DateTime.SpecifyKind(localDate.AddHours(-7), DateTimeKind.Utc);
            var endOfDayUtc = startOfDayUtc.AddDays(1).AddTicks(-1);

            return await FindByCondition(x => x.NailArtistId == artistId 
                                              && x.WorkDate >= startOfDayUtc 
                                              && x.WorkDate <= endOfDayUtc 
                                              && (x.Status == "Available" || x.Status == "Active"))
                                  .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Schedule>> GetSchedulesByArtistIdAsync(Guid artistId, DateTime? startDate, DateTime? endDate)
        {
            var query = FindByCondition(s => s.NailArtistId == artistId
                && (s.Status == "Available" || s.Status == "Active"));

            if (startDate.HasValue)
                query = query.Where(s => s.WorkDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(s => s.WorkDate <= endDate.Value);

            return await query
                .OrderBy(s => s.WorkDate)
                .ThenBy(s => s.ShiftStart)
                .ToListAsync();
        }

        public async Task<IEnumerable<Schedule>> GetSchedulesBySalonIdAsync(Guid salonId, DateTime? startDate, DateTime? endDate)
        {
            var query = FindByCondition(x => x.NailArtist.Account.SalonId == salonId
                && (x.Status == "Available" || x.Status == "Active"));

            if (startDate.HasValue)
                query = query.Where(x => x.WorkDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(x => x.WorkDate <= endDate.Value);

            return await query
                .OrderBy(x => x.WorkDate)
                .ThenBy(x => x.ShiftStart)
                .ToListAsync();
        }
    }
}
