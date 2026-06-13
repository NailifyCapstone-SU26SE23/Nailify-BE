using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Domain.Entities
{
    public class Services
    {
        public Guid ServiceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public string Status { get; set; }
        public DateTime CreateAt { get; set; }

        public virtual ICollection<BookingItem> BookingItems { get; set; } = new List<BookingItem>();
    }
}
