namespace Nailify.Capstone.Domain.Entities
{
    public class BookingDiscount
    {
        public int BookingDiscountId { get; set; }
        public Guid BookingId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal DiscountAmount { get; set; }
        public bool IsAutoApplied { get; set; }
        public DateTime AppliedDate { get; set; } = DateTime.UtcNow;
        public int? PromotionId { get; set; }
        public int? LoyaltyTierId { get; set; }
        public int? LoyaltyTransactionId { get; set; }

        public virtual Booking Booking { get; set; } = null!;
        public virtual Promotion? Promotion { get; set; }
        public virtual LoyaltyTier? LoyaltyTier { get; set; }
        public virtual LoyaltyTransaction? LoyaltyTransaction { get; set; }
    }
}
