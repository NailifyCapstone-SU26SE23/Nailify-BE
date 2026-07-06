using System;
using System.Collections.Generic;

namespace Nailify.Capstone.Domain.Entities
{
    public class Chair
    {
        public Guid ChairId { get; set; }
        public Guid SalonId { get; set; }
        public string ChairName { get; set; } = null!;
        public string Status { get; set; } = "Active"; // Active, Maintenance, Inactive

        public virtual Salon Salon { get; set; } = null!;
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
