namespace Nailify.Capstone.Domain.Entities
{
    public class FavoriteNail
    {
        public int FavoriteNailId { get; set; }
        public Guid UserId { get; set; }

        // Both references
        public int? NailDesignId { get; set; }
        public int? NailVariantId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual NailDesign? NailDesign { get; set; }
        public virtual NailVariant? NailVariant { get; set; }
        public virtual User User { get; set; } = null!;
    }
}
