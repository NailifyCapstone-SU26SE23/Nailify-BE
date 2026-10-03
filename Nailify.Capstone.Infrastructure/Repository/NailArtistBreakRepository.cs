using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Infrastructure.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nailify.Capstone.Domain.Enums;
namespace Nailify.Capstone.Infrastructure.Repository
{
    public class NailArtistBreakRepository : GenericRepository<NailArtistBreak>, INailArtistBreakRepository
    {
        public NailArtistBreakRepository(NailifyDbContext context) : base(context)
        {
        }

        public async Task<List<NailArtistBreak>> GetApprovedBreaksByArtistAndDateAsync(Guid artistId, DateTime date)
        {
            var targetDate = (date.Kind == DateTimeKind.Utc ? date.AddHours(7) : date).Date;
            return await FindByCondition(x => x.NailArtistId == artistId
                                         && x.BreakDate.Date == targetDate
                                         && x.Status == ArtistBreakStatus.Approved)
                         .ToListAsync();
        }

        public async Task<List<NailArtistBreak>> GetApprovedBreaksByArtistIdsAndDateAsync(IEnumerable<Guid> artistIds, DateTime date)
        {
            var idList = artistIds.Distinct().ToList();
            if (!idList.Any())
            {
                return new List<NailArtistBreak>();
            }
            var targetDate = (date.Kind == DateTimeKind.Utc ? date.AddHours(7) : date).Date;

            return await FindByCondition(
                                         x => idList.Contains(x.NailArtistId)
                                         && x.BreakDate.Date == targetDate
                                         && x.Status == ArtistBreakStatus.Approved
                                        )
                        .ToListAsync();
        }
    }
}
