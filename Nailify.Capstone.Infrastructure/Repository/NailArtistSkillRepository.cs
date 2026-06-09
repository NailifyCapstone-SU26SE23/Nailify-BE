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
    public class NailArtistSkillRepository : GenericRepository<NailArtistSkill>, INailArtistSkillRepository
    {
        public NailArtistSkillRepository(NailifyDbContext context) : base(context)
        {
        }

        public async Task<NailArtistSkill?> GetByArtistAndSkillAsync(Guid artistId, Guid skillId) 
            => await FindByCondition(x => x.NailArtistId == artistId && x.SkillTypeId == skillId)
                                    .Include(x => x.SkillType)
                                    .FirstOrDefaultAsync();

        public Task<List<NailArtistSkill>> GetSkillsByArtistIdAsync(Guid artistId)
            => FindByCondition(x => x.NailArtistId == artistId)
                .Include(x => x.SkillType)
                .ToListAsync();
    }
}
