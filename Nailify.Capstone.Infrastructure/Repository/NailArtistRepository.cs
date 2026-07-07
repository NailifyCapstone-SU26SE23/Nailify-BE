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
    public class NailArtistRepository : GenericRepository<NailArtist>, INailArtistRepository
    {
        public NailArtistRepository(NailifyDbContext context) : base(context) { }

        public async Task<IEnumerable<NailArtist>> GetNailArtistsBySalonIdAsync(Guid salonId)
        {
            return await FindByCondition(x => x.Account.SalonId == salonId && x.Status == "Active")
                .Include(x => x.Account)
                    .ThenInclude(x => x.Salon)
                .ToListAsync();
        }

        public async Task<NailArtist?> GetNailArtistWithProfileAsync(Guid artistId)
        {
            return await FindByCondition(na => na.NailArtistId == artistId && na.Status == "Active")
                .Include(na => na.Account)
                .FirstOrDefaultAsync();
        }

        public async Task<NailArtist?> GetNailArtistByAccountIdAsync(Guid accountId)
        {
            return await FindByCondition(na => na.AccountId == accountId && na.Status == "Active")
                .Include(na => na.Account)
                .FirstOrDefaultAsync();
        }

        public async Task<List<NailArtist>> GetSuggestedArtistsAsync(Guid salonId, List<int> nailVariantIds)
        {
            var requiredSkills = await _context.NailRequiredSkills
                                               .Where(x => nailVariantIds.Contains(x.NailVariantId))
                                               .ToListAsync();
            var artists = await FindByCondition(x => x.Account.SalonId == salonId && x.Status == "Active")
                                               .Include(x => x.Account)
                                                    .ThenInclude(x => x.Salon)
                                               .Include(x => x.NailArtistSkills)
                                                    .ThenInclude(nas => nas.SkillType)
                                               .ToListAsync();

            var suggestedArtists = new List<NailArtist>();
            foreach (var artist in artists)
            {
                bool isQualified = true;
                foreach (var reqSkill in requiredSkills)
                {
                    var artistSkill = artist.NailArtistSkills.FirstOrDefault(x => x.SkillTypeId == reqSkill.SkillTypeId);
                    if (artistSkill == null || artistSkill.Level < reqSkill.RequiredLevel)
                    {
                        isQualified = false;
                        break;
                    }
                }
                if (isQualified)
                {
                    suggestedArtists.Add(artist);
                }
            }
            return suggestedArtists;
        }

        public async Task<NailArtist?> GetArtistWithLockAsync(Guid artistId)
        {
            // Sử dụng Raw SQL SELECT FOR UPDATE của PostgreSQL để thực hiện khóa dòng
            return await _context.NailArtists
                .FromSqlRaw("SELECT * FROM \"NailArtists\" WHERE \"NailArtistId\" = {0} FOR UPDATE", artistId)
                .Include(x => x.Account)
                .FirstOrDefaultAsync();
        }

        public async Task<List<NailArtist>> GetArtistsWithSkillsBySalonIdAsync(Guid salonId)
        {
            return await FindByCondition(x => x.Account.SalonId == salonId && x.Status == "Active", false)
                .Include(x => x.Account)
                .Include(x => x.NailArtistSkills)
                .ToListAsync();
        }
    }
}
