using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Domain.Entities
{
    public class NailArtistSkill
    {
        public Guid NailArtistSkillId { get; set; }
        public Guid NailArtistId { get; set; }
        public Guid SkillTypeId { get; set; }
        public int Level { get; set; }
        public virtual NailArtist NailArtist { get; set; }
        public virtual SkillType SkillType { get; set; }
    }
}
