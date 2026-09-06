namespace Nailify.Capstone.Domain.Entities
{
    public class LoyaltyTier
    {
        public int LoyaltyTierId { get; set; }
        public string Name { get; set; } = string.Empty; // Bronze, Silver, Gold, Platinum, Diamond
        public string Description { get; set; } = string.Empty;
        public int? MinLifetimePoints { get; set; }
        public int? MaxLifetimePoints { get; set; }

        // Benefits
        public decimal DiscountRate { get; set; } = 0m;

        // 🎨 Visual Styling Properties
        public string? ImageUrl { get; set; } // e.g., "/images/tiers/diamond-banner.jpg"

        public string? BackgroundColor { get; set; } // e.g., "#FFD700" for Gold
        public string? TextColor { get; set; } // e.g., "#FFFFFF" for white text

        public string? ColorJson { get; set; } // JSON for multiple colors/theme

        public string Status { get; set; } = "Active";
        public int? SortOrder { get; set; }

        // Navigation
        public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
    }
}