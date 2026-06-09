using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Domain.Entities
{
    public class BookingItem
    {
        public Guid BookingItemId { get; set; }
        public Guid BookingId { get; set; }
        public Guid? ServiceId { get; set; }
        public int? NailVariantId { get; set; }
        public int? CustomerNailId { get; set; }

        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public virtual Booking Booking { get; set; } = null!;
        public virtual Services? Service { get; set; }
        public virtual NailVariant? NailVariant { get; set; }
        public virtual CustomerNail? CustomerNail { get; set; }
    }
}
