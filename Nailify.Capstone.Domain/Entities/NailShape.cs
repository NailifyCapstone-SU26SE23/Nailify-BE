namespace Nailify.Capstone.Domain.Entities
{
    public class NailShape
    {
        public int NailShapeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int? Duration { get; set; }

        public virtual ICollection<NailVariant> NailVariants { get; set; } = new List<NailVariant>();
    }
}
