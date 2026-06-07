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
        public Guid SalonId { get; set; }
        public string Status { get; set; }
        public virtual User Account { get; set; } = null!;
        public virtual Salon Salon { get; set; } = null!;
        public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

        public virtual ICollection<NailArtistSkill> NailArtistSkills { get; set; }
    }
}
