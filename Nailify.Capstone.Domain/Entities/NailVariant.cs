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
        public string ImageUrl { get; set; } = string.Empty;        

        public virtual NailShape NailShape { get; set; } = null!;
        public virtual NailSurface NailSurface { get; set; } = null!;
        public virtual NailDesign NailDesign { get; set; } = null!;
        public virtual ICollection<NailComponent> NailComponents { get; set; } = new List<NailComponent>();
    }
}
