namespace Nailify.Capstone.Domain.Entities
{
    public class NailComponent
    {
        public int NailComponentId { get; set; }
        public int ComponentId { get; set; }
        public int NailVariantId { get; set; }
        public decimal PosX { get; set; }
        public decimal PosY { get; set; }
        public int FingerIndex { get; set; } = -1;
        public string ConfigJson { get; set; } = string.Empty;

        public virtual Component Component { get; set; } = null!;
        public virtual NailVariant NailVariant { get; set; } = null!;
    }
}
