namespace Nailify.Capstone.Domain.Entities
{
    public class NailSurface
    {
        public int NailSurfaceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ShaderParam { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int? Duration { get; set; }

        public virtual ICollection<NailVariant> NailVariants { get; set; } = new List<NailVariant>();
    }
}
