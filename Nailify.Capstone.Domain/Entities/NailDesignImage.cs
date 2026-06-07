namespace Nailify.Capstone.Domain.Entities
{
    public class NailDesignImage
    {
        public int NailDesignImageId { get; set; }
        public int NailDesignId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;

        public virtual NailDesign NailDesign { get; set; } = null!;
    }
}
