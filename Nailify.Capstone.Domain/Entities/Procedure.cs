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
        // New
        public int ActiveDuration { get; set; }
        private int _passiveDuration;
        public int PassiveDuration
        {
            get => _passiveDuration;
            set
            {
                _passiveDuration = value;
                // Tự động set CanOverlap = true nếu PassiveDuration >= 4 phút
                CanOverlap = _passiveDuration >= 4;
            }
        }
        public bool CanOverlap { get; set; }
        public int TransitionBuffer { get; set; } = 1; // Thời gian đệm
        public string Status { get; set; } = "Active";
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public bool IsRequired { get; set; } = true;
        public bool IsMainStep { get; set; } = true;
        public virtual ICollection<NailProcedure> NailProcedures { get; set; } = new List<NailProcedure>();
    }
}
