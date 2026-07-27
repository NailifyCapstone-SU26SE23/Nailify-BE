using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Domain.Entities
{
    public class WalkInQueue
    {
        public Guid QueueId { get; set; }
        public Guid SalonId { get; set; }
        public Guid? CustomerId { get; set; } // null nếu khách vãng lai
        public Guid? OriginalBookingId { get; set; } // nếu là late arrival có booking
        public Guid? ChairId { get; set; }
        public string? GuestName { get; set; } // khách vãng lai không có account
        public string? GuestPhone { get; set; }
        public int QueuePosition { get; set; } // số thứ tự tại quầy
        public QueueStatus Status { get; set; }
        public DateTime ArrivalTime { get; set; }
        public DateTime? CalledTime { get; set; }
        public DateTime? ServiceStartTime { get; set; }
        public Guid? AssignedNailArtistId { get; set; }
        public string? RequestNote { get; set; }        // khách muốn làm gì
        public int? EstimatedWait { get; set; }         // phút chờ ước tính
        public virtual Salon Salon { get; set; } = null!;
        public virtual Customer? Customer { get; set; }
        public virtual Booking? OriginalBooking { get; set; }
        public virtual NailArtist? AssignedNailArtist { get; set; }
        public virtual Chair? Chair { get; set; }
    }
}
