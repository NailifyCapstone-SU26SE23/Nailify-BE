namespace Nailify.Capstone.Domain.Entities
{
    public class Component
    {
        public int ComponentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public ComponentType ComponentType { get; set; }
        public decimal Price { get; set; }

        public virtual ICollection<NailComponent> NailComponents { get; set; } = new List<NailComponent>();
    }
}
