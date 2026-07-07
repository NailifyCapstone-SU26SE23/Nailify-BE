namespace Nailify.Capstone.Domain.Entities
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CategoryTypeId { get; set; }
        public string Status { get; set; } = "Active";

        public virtual CategoryType CategoryType { get; set; } = null!;
        public virtual ICollection<NailCategory> NailCategories { get; set; } = new List<NailCategory>();
        public virtual ICollection<Promotion> Promotions { get; set; } = new List<Promotion>();
    }
}
