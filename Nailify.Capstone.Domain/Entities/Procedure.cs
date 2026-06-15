using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Domain.Entities
{
    public class Procedure
    {
        public Guid ProcedureId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int? Duration { get; set; }
        public string Status { get; set; } = "Active";
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public virtual ICollection<NailProcedure> NailProcedures { get; set; } = new List<NailProcedure>();
    }
}
