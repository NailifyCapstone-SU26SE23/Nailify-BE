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

        public async Task<IEnumerable<Schedule>> GetSchedulesByArtistIdAsync(Guid artistId, DateTime? startDate, DateTime? endDate)
        {
            var query = FindByCondition(s => s.NailArtistId == artistId);

            if (startDate.HasValue)
                query = query.Where(s => s.WorkDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(s => s.WorkDate <= endDate.Value);

            return await query
                .OrderBy(s => s.WorkDate)
                .ThenBy(s => s.ShiftStart)
                .ToListAsync();
        }
    }
}
