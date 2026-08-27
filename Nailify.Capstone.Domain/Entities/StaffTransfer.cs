using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Domain.Entities
{
    public class StaffTransfer
    {
        public Guid StaffTransferId { get; set; }
        public Guid NailArtistId { get; set; }
        public Guid FromSalonId { get; set; }
        public Guid ToSalonId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public NailArtistTransferStatus Status { get; set; } = NailArtistTransferStatus.Scheduled;
        public string? Reason { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public virtual NailArtist NailArtist { get; set; } = null!;
        public virtual Salon FromSalon { get; set; } = null!;
        public virtual Salon ToSalon { get; set; } = null!; 
    }
}
