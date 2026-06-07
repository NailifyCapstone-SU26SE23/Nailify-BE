using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Domain.Entities
{
    public class Schedule
    {
        public Guid ScheduleId { get; set; }
        public Guid NailArtistId { get; set; }
        public DateTime WorkDate { get; set; }
        public TimeSpan ShiftStart { get; set; }
        public TimeSpan ShiftEnd { get; set; }
        public string Status { get; set; }
        public virtual NailArtist NailArtist { get; set; }
    }
}
