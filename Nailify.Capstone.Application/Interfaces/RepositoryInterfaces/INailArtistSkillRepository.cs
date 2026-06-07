using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface INailArtistSkillRepository : IGenericRepository<NailArtistSkill>
    {
        Task<List<NailArtistSkill>> GetSkillsByArtistIdAsync(Guid artistId);
        Task<NailArtistSkill?> GetByArtistAndSkillAsync(Guid artistId, Guid skillId);
    }
}
