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
        public Guid? ManagerId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Status { get; set; }
        public string? ImageUrl { get; set; }
        public virtual ICollection<SalonOperatingHour> OperatingHours { get; set; }
        public virtual ICollection<NailArtist> NailArtists { get; set; } = new List<NailArtist>();
        public virtual User? Manager
        { get; set; }
    } 
}
