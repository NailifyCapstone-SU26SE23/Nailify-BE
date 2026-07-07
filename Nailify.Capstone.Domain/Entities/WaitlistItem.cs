using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Domain.Entities
{
    public class WaitlistItem
    {
        public Guid WaitlistItemId { get; set; }
        public Guid WaitlistId { get; set; }
        public int? NailVariantId { get; set; }
        public Guid? ServiceId { get; set; }
        public int? CustomerNailId { get; set; } 
        public int Quantity { get; set; } = 1;
        public virtual BookingWaitlist BookingWaitlist { get; set; } = null!;
        public virtual NailVariant? NailVariant { get; set; }
        public virtual Nailify.Capstone.Domain.Entities.Services? Service { get; set; }
        public virtual CustomerNail? CustomerNail { get; set; }

    }
}
