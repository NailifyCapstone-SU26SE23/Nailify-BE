namespace Nailify.Capstone.Domain.Entities
{
    public class UserPromotionUsage
    {
        public int UserPromotionUsageId { get; set; }
        public Guid UserId { get; set; }
        public int PromotionId { get; set; }
        public int UsageCount { get; set; }
        public DateTime LastUsedDate { get; set; } = DateTime.UtcNow;
        public int? ReceivedCount { get; set; }       
        public virtual User User { get; set; } = null!;
        public virtual Promotion Promotion { get; set; } = null!;
    }
}
