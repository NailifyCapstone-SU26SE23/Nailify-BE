using Nailify.Capstone.Domain.Common.Events;
using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Domain.Entities
{
    public class BookingWaitlist : EventEntity
    {
        public Guid WailistId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid SalonId { get; set; }
        public Guid? PreferredNailArtistId { get; set; }

        [Column("RequesetedDate")]
        public DateTime RequestedDate { get; set; }
        public TimeSpan RequestedStartTime { get; set; }
        public int EstimatedDuration { get; set; }
        public int Position { get; set; }
        public WaitlistStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? NotifiedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }     
        public Guid? ConvertedBookingId { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Salon Salon { get; set; }
        public virtual NailArtist? PreferredNailArtist { get; set; }
        public virtual Booking? ConvertedBooking { get; set; }
        public virtual ICollection<WaitlistItem> WaitlistItems { get; set; } = new List<WaitlistItem>();
    }
}
