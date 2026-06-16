using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Domain.Entities
{
    public class NailProcedure
    {
        public Guid NailProcedureId { get; set; }
        public int NailVariantId { get; set; }
        public Guid ProcedureId { get; set; }
        public int StepOrder { get; set; }
        public string Status { get; set; } = "Active";
        public virtual NailVariant NailVariant { get; set; } = null!;
        public virtual Procedure Procedure { get; set; } = null!;
    }
}
