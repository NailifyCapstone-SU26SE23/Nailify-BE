namespace Nailify.Capstone.Domain.Entities
{
    public class ShapeMethodConfig
    {
        public int ShapeMethodConfigId { get; set; }

        public int NailShapeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Duration { get; set; } 
        public string Status { get; set; } = "Active";

        public virtual NailShape NailShape { get; set; } = null!;
    }
}