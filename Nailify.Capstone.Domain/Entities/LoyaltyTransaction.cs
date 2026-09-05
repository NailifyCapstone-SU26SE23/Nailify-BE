using Nailify.Capstone.Domain.Enums;
namespace Nailify.Capstone.Domain.Entities
{
    public class LoyaltyTransaction
    {
        public int LoyaltyTransactionId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? BookingId { get; set; }

        public int Points { get; set; }
        public LoyaltyTransactionType TransactionType { get; set; } = LoyaltyTransactionType.Earned;

        //public int? LoyaltyTierIdAtTime { get; set; } // Track which tier they were in
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual Customer Customer { get; set; } = null!;
        public virtual Booking? Booking { get; set; }
        //public virtual LoyaltyTier? LoyaltyTier { get; set; }
    }
}
