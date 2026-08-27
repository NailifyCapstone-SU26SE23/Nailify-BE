using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Domain.Entities
{
    public class SalonOffDate
    {
        public Guid SalonOffDateId { get; set; } = Guid.NewGuid();
        public Guid SalonId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Description { get; set; }
        public virtual Salon Salon { get; set; } = null!;
    }
}
