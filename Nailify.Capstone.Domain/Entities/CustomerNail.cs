using Nailify.Capstone.Domain.Enums;

namespace Nailify.Capstone.Domain.Entities
{
    public class CustomerNail
    {
        public int CustomerNailId { get; set; }
        public Guid UserId { get; set; }
        public Guid? SalonId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int? NailShapeId { get; set; }
        public int? NailSurfaceId { get; set; }
        public decimal? Price { get; set; }
        public string? CustomColor { get; set; }
        public int? Duration { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsPublic { get; set; } = false;
        //public string Status { get; set; } = "Active";
        public CustomerNailStatus Status { get; set; } = CustomerNailStatus.Draft;
        public string? RejectReason { get; set; }
        public Guid? ApprovedArtistId { get; set; }
        public virtual NailArtist? ApprovedArtist { get; set; }
        public virtual User User { get; set; } = null!;
        public virtual NailShape NailShape { get; set; } = null!;
        public virtual NailSurface NailSurface { get; set; } = null!;
        public virtual Salon? Salon { get; set; }
        public virtual ICollection<CustomerNailComponent> CustomerNailComponents { get; set; } = new List<CustomerNailComponent>();
    }
}
