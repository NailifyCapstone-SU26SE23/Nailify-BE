using Nailify.Capstone.Domain.Enums;

namespace Nailify.Capstone.Domain.Entities
{
    public class CustomerNail
    {
        public int CustomerNailId { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int? NailShapeId { get; set; }
        public int? NailSurfaceId { get; set; }
        public decimal? Price { get; set; }
        public string? CustomColor { get; set; }
        public int? Duration { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsPublic { get; set; } = false;
        public string Status { get; set; } = "Active";
        public virtual User User { get; set; } = null!;
        public virtual NailShape NailShape { get; set; } = null!;
        public virtual NailSurface NailSurface { get; set; } = null!;
        public virtual ICollection<CustomerNailComponent> CustomerNailComponents { get; set; } = new List<CustomerNailComponent>();
        public virtual ICollection<CustomerNailRequest> CustomerNailRequests { get; set; } = new List<CustomerNailRequest>();
    }
}
