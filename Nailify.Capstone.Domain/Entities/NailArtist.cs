using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Domain.Entities
{
    public class NailArtist
    {
        public Guid NailArtistId { get; set; }
        public Guid AccountId { get; set; }
        public int ConcurrentCapacity { get; set; } = 1;
        public string Status { get; set; } = "Active";
        public virtual User Account { get; set; } = null!;
        public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

        public virtual ICollection<NailArtistSkill> NailArtistSkills { get; set; }
        public virtual ICollection<NailArtistBreak> NailArtistBreaks { get; set; } = new List<NailArtistBreak>();
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
