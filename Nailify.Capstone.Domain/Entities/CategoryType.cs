namespace Nailify.Capstone.Domain.Entities
{
    public class CategoryType
    {
        public int CategoryTypeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";

        public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
    }
}
