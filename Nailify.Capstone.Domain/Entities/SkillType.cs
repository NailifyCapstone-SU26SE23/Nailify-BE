using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Domain.Entities
{
    public class SkillType
    {
        public Guid SkillTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } = "Active";

        public virtual ICollection<NailArtistSkill> NailArtistSkills { get; set; }
        public virtual ICollection<NailRequiredSkill> NailRequiredSkills { get; set; }
    }
}
