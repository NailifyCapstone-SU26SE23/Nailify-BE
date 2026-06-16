using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Domain.Entities
{
    public class BookingHistory
    {
        public Guid BookingHistoryId { get; set; }
        public Guid BookingId { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string? Payload { get; set; }
        public Guid? ActorId { get; set; }
        public virtual User? Actor { get; set; }
        public virtual Booking Booking { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
