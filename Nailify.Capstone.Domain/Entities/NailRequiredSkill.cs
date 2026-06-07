using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Domain.Entities
{
    public class NailRequiredSkill
    {
        public Guid NailRequiredSkillId { get; set; }
        public int NailVariantId { get; set; }
        public Guid SkillTypeId { get; set; }
        public int RequiredLevel { get; set; }
        public virtual NailVariant NailVariant { get; set; }
        public virtual SkillType SkillType { get; set; }
    }
}
