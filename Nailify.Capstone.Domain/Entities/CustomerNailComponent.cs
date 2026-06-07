namespace Nailify.Capstone.Domain.Entities
{
    public class CustomerNailComponent
    {
        public int CustomerNailComponentId { get; set; }
        public int CustomerNailId { get; set; }
        public int? ComponentId { get; set; }
        public int? CustomerComponentId { get; set; }
        public decimal PosX { get; set; }
        public decimal PosY { get; set; }
        public int FingerIndex { get; set; } = -1;
        public string ConfigJson { get; set; } = string.Empty;

        public virtual CustomerNail CustomerNail { get; set; } = null!;
        public virtual Component? Component { get; set; }
        public virtual CustomerComponent? CustomerComponent { get; set; }
    }
}
