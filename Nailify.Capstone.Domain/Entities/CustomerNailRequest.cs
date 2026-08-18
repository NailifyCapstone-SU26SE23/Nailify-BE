using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Domain.Entities
{
    public class CustomerNailRequest
    {
        public Guid CustomerNailRequestId { get; set; }
        public int CustomerNailId { get; set; }
        public Guid SalonId { get; set; }
        public CustomerNailStatus Status { get; set; } = CustomerNailStatus.PendingReview;
        public string? RejectReason { get; set; }
        public Guid? ApprovedArtistId { get; set; }
        public decimal? Price { get; set; } // Giá chốt cuối cùng cho request này
        public int? Duration { get; set; }  // Thời gian làm chốt cho request này
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsCustomerRequest { get; set; } = false; 
        public virtual CustomerNail CustomerNail { get; set; } = null!;
        public virtual Salon Salon { get; set; } = null!;
        public virtual NailArtist? ApprovedArtist { get; set; }
    }
}
