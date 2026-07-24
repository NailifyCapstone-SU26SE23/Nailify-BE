using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Domain.Entities
{
    public class NailArtistBreak
    {
        public Guid NailArtistBreakId { get; set; }
        public Guid NailArtistId { get; set; }
        public DateTime BreakDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string? Reason { get; set; }
        public string? RejectReason { get; set; }
        public ArtistBreakStatus Status { get; set; } = ArtistBreakStatus.Approved;
        public virtual NailArtist NailArtist { get; set; }
    }
}
