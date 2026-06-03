namespace Nailify.Capstone.Domain.Entities
{
    public class CustomerComponent
    {
        public int CustomerComponentId { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public ComponentType ComponentType { get; set; }
        public decimal? Price { get; set; }
        public string CustomDataJson { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsPublic { get; set; } = false;

        public virtual User User { get; set; } = null!;
        public virtual ICollection<CustomerNailComponent> CustomerNailComponents { get; set; } = new List<CustomerNailComponent>();
    }
}
