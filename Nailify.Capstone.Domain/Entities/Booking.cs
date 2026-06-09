using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Domain.Entities
{
    
    public class Booking
    {
        public Guid BookingId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid SalonId { get; set; }
        public Guid? NailArtistId { get; set; }
        public DateTime BookingDate { get; set; }
        public TimeSpan ExpectedTime { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public string Price { get; set; }
        public int TotalDuration { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CheckInImageUrl { get; set; }
        public string? CheckOutImagesUrl { get; set; }
        public string? QRCode { get; set; }
        public virtual Customer Customer { get; set; } = null!;
        public virtual Salon Salon { get; set; } = null!;
        public virtual NailArtist? NailArtist { get; set; }
        public virtual ICollection<BookingItem> BookingItems { get; set; } = new List<BookingItem>();
        public virtual ICollection<BookingHistory> BookingHistories { get; set; } = new List<BookingHistory>();
    }
}
