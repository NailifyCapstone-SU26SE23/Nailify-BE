using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Domain.Entities
{
    public class Salon
    {
        public Guid SalonId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Status { get; set; }
        public string? ImageUrl { get; set; }
        public decimal DepositConfig { get; set; } = 0.2m; 
        public virtual ICollection<SalonOperatingHour> OperatingHours { get; set; }
        public virtual ICollection<Chair> Chairs { get; set; } = new List<Chair>();
        public virtual ICollection<SalonOffDate> OffDates { get; set; } = new List<SalonOffDate>();
    }
}
