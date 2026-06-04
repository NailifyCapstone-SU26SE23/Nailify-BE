namespace Nailify.Capstone.Domain.Entities
{
    public class NailCategory
    {
        public int NailCategoryId { get; set; }
        public int NailDesignId { get; set; }
        public int CategoryId { get; set; }

        public virtual NailDesign NailDesign { get; set; } = null!;
        public virtual Category Category { get; set; } = null!;
    }
}
