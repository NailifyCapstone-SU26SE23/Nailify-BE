using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Common.Models.Scheduling
{
    public sealed class ProcedureScheduleSegment
    {
        public Guid BookingProcedureId { get; set; }
        public Guid? BookingItemId { get; set; }
        public Guid? BookingId { get; set; }
        public Guid? AssignedArtistId { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public TimeSpan ArtistBusyStart { get; set; }
        public TimeSpan ArtistBusyEnd { get; set; }

        public bool CanOverlap { get; set; }
        public int TransitionBuffer { get; set; } = 1;
        public bool IsMainStep { get; set; } = true;
    }
}
