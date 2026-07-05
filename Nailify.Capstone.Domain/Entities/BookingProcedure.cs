using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Domain.Entities
{
    public class BookingProcedure
    {
        public Guid BookingProcedureId { get; set; }
        public Guid BookingItemId { get; set; }
        public Guid? ProcedureId { get; set; }
        public string ProcedureName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int StepOrder { get; set; }
        public BookingProcedureStatus Status { get; set; } = BookingProcedureStatus.Pending;
        // New snapshot fields
        public int Duration { get; set; }
        public int ActiveDuration { get; set; }
        public int PassiveDuration { get; set; }
        public bool CanOverlap { get; set; }
        // New scheduling fields
        public TimeSpan? EstimatedStartTime { get; set; }
        public TimeSpan? EstimatedEndTime { get; set; }
        public DateTime? ActualStartTime { get; set; }
        public DateTime? ActualEndTime { get; set; }
        public Guid? AssignedArtistId { get; set; }
        public DateTime? CompletedAt { get; set; }
        public Guid? CompletedById { get; set; }
        public bool IsRequired { get; set; } = true;

        public virtual BookingItem BookingItem { get; set; } = null!;
        public virtual Procedure? Procedure { get; set; }
        public virtual NailArtist? CompletedBy { get; set; }
        public virtual NailArtist? AssignedArtist { get; set; }
    }
}
