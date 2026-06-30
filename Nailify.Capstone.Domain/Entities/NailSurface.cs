namespace Nailify.Capstone.Domain.Entities
{
    public class NailSurface
    {
        public int NailSurfaceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ShaderParam { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int? Duration { get; set; }

        public float LightnessOffset { get; set; } = 0.0f; // -1.0 to 1.0
        public float SaturationOffset { get; set; } = 0.0f; // -1.0 to 1.0
        public float HueOffset { get; set; } = 0.0f; // -180 to 180

        public string FinishType { get; set; } = "glossy"; 
        // matte, glossy, chrome, catEye, holographic, pearl, satin, glitter

        public string Status { get; set; } = "Active";

        public virtual ICollection<NailVariant> NailVariants { get; set; } = new List<NailVariant>();
    }
}
