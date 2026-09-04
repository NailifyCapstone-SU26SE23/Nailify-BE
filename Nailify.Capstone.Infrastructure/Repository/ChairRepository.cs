using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Infrastructure.DBContext;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nailify.Capstone.Infrastructure.Repository
{
    public class ChairRepository : GenericRepository<Chair>, IChairRepository
    {
        public ChairRepository(NailifyDbContext context) : base(context)
        {
        }

        public async Task<Chair?> GetChairWithSalonAsync(Guid chairId)
        {
            return await FindByCondition(c => c.ChairId == chairId)
                .Include(c => c.Salon)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Chair>> GetActiveChairsBySalonAsync(Guid salonId)
        {
            return await FindByCondition(c => c.SalonId == salonId && c.Status == "Active")
                .Include(c => c.Salon)
                .ToListAsync();
        }

        public async Task<PagedList<Chair>> GetPagedChairsBySalonAsync(Guid salonId, int pageNumber, int pageSize, string? statusFilter = null,
          string? orderBy = null)
        {
            return await GetPagedAsync(
                pageNumber,
                pageSize,
                c => c.SalonId == salonId,
                statusFilter,
                orderBy,
                c => c.Salon
            );
        }
    }
}
