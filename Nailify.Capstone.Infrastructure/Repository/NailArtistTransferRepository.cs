using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.Common;
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
    public class NailArtistTransferRepository : GenericRepository<StaffTransfer>, INailArtistTransferRepository
    {
        public NailArtistTransferRepository(NailifyDbContext context) : base(context)
        {
        }

        public async Task<StaffTransfer?> GetActiveTransferByArtistAndDateAsync(Guid artistId, DateTime date)
        {
            var localDate = date.Date;
            return await FindByCondition(x => x.NailArtistId == artistId
                                         && x.Status == NailArtistTransferStatus.Scheduled
                                         && x.StartDate <= localDate
                                         && x.EndDate >= localDate)
                       .FirstOrDefaultAsync();
        }

        public async Task<PagedList<StaffTransfer>> GetPagedTransferAsync(int pageNumber, int pageSize, Guid? salonId, Guid? artistId, NailArtistTransferStatus? status)
        {
            var query = FindByCondition(x =>
                                        (!salonId.HasValue || x.FromSalonId == salonId.Value || x.ToSalonId == salonId.Value) 
                                        && (!artistId.HasValue || x.NailArtistId == artistId.Value)
                                        && (!status.HasValue || x.Status == status.Value)
                                       )
                       .Include(x => x.NailArtist)
                           .ThenInclude(x => x.Account)
                       .Include(x => x.FromSalon)
                       .Include(x => x.ToSalon)
                       .OrderByDescending(x => x.CreatedAt);

            var count = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedList<StaffTransfer>(items, count, pageNumber, pageSize);
        }

        public async Task<List<NailArtist>> GetTransferredOutArtistIdsAsync(Guid salonId, DateTime date)
        {
            var localDate = date.Date;
            var transfer = await FindByCondition(x => x.FromSalonId == salonId
                                         && x.Status == NailArtistTransferStatus.Scheduled
                                         && x.StartDate <= localDate
                                         && x.EndDate >= localDate)
                        .Include(x => x.NailArtist)
                            .ThenInclude(x => x.Account)
                        .ToListAsync();
            return transfer.Select(x => x.NailArtist).ToList();
        }

        public Task<List<StaffTransfer>> GetTransfersIntoSalonByDateAsync(Guid salonId, DateTime date)
        {
            var localDate = date.Date;
            return FindByCondition(x => x.ToSalonId == salonId
                                         && x.Status == NailArtistTransferStatus.Scheduled
                                         && x.StartDate <= localDate
                                         && x.EndDate >= localDate)
                    .Include(x => x.NailArtist)
                        .ThenInclude(x => x.Account)
                    .Include(x => x.NailArtist)
                        .ThenInclude(x => x.NailArtistSkills)
                    .Include(x => x.NailArtist)
                        .ThenInclude(x => x.Schedules)
                    .Include(x => x.NailArtist)
                        .ThenInclude(x => x.NailArtistBreaks)
                    .ToListAsync();
        }

        public async Task<bool> HasOverlappingTransferAsync(Guid artistId, DateTime startDate, DateTime endDate)
        {
            var s = startDate.Date;
            var e = endDate.Date;
            return await FindByCondition(x => x.NailArtistId == artistId
                                         && x.Status == NailArtistTransferStatus.Scheduled
                                         && x.StartDate <= e
                                         && x.EndDate >= s)
                   .AnyAsync();
        }
    }
}
