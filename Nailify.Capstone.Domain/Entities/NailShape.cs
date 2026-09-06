namespace Nailify.Capstone.Domain.Entities
{
    public class NailShape
    {
        public int NailShapeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";
        public virtual ICollection<ShapeMethodConfig> ShapeMethodConfigs { get; set; } = new List<ShapeMethodConfig>();
        public virtual ICollection<NailVariant> NailVariants { get; set; } = new List<NailVariant>();
    }
}
