namespace Nailify.Capstone.Domain.Entities
{
    public class NailVariant
    {
        public int NailVariantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int NailShapeId { get; set; }
        public int NailSurfaceId { get; set; }
        public int NailDesignId { get; set; }
        public decimal Price { get; set; }
        public int? Duration { get; set; }
        public decimal? Precision { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Form { get; set; } = string.Empty;
        public string Material { get; set; } = string.Empty;
        public int? Speed { get; set; }

        public virtual NailShape NailShape { get; set; } = null!;
        public virtual NailSurface NailSurface { get; set; } = null!;
        public virtual NailDesign NailDesign { get; set; } = null!;
        public virtual ICollection<NailComponent> NailComponents { get; set; } = new List<NailComponent>();
    }
}
