namespace Nailify.Capstone.Domain.Entities
{
    public class NailDesign
    {
        public int NailDesignId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";
        public virtual ICollection<NailCategory> NailCategories { get; set; } = new List<NailCategory>();
        public virtual ICollection<NailDesignImage> NailDesignImages { get; set; } = new List<NailDesignImage>();
        public virtual ICollection<NailVariant> NailVariants { get; set; } = new List<NailVariant>();
        public virtual ICollection<Promotion> Promotions { get; set; } = new List<Promotion>();
    }
}
