using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface INailRequiredSkillRepository : IGenericRepository<NailRequiredSkill>
    {
        Task<List<NailRequiredSkill>> GetSkillsByDesignIdAsync(int designId);
         Task<NailRequiredSkill> GetByNailDesignAndSkillAsync(int nailDesignId, Guid skillTypeId);
    }
}
