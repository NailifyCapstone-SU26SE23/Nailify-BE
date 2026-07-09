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
    public class NailRequiredSkillRepository : GenericRepository<NailRequiredSkill>, INailRequiredSkillRepository
    {
        public NailRequiredSkillRepository(NailifyDbContext context) : base(context)
        {
        }

        public async Task<NailRequiredSkill> GetByNailDesignAndSkillAsync(int nailDesignId, Guid skillTypeId)
          => await FindByCondition(x => x.NailVariantId == nailDesignId && x.SkillTypeId == skillTypeId)
                                  .Include(x => x.SkillType)
                                  .FirstOrDefaultAsync();

        public Task<List<NailRequiredSkill>> GetSkillsByDesignIdAsync(int designId)
           => FindByCondition(x => x.NailVariantId == designId)
                             .Include(x => x.SkillType)
                             .ToListAsync();

        public Task<List<NailRequiredSkill>> GetSkillsByVariantIdsAsync(List<int> variantIds)
          => FindByCondition(x => variantIds.Contains(x.NailVariantId))
                             .Include(x => x.SkillType)
                             .ToListAsync();
    }
}
