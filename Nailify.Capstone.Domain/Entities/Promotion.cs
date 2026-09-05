using Nailify.Capstone.Domain.Enums;

namespace Nailify.Capstone.Domain.Entities
{
    public class Promotion
    {
        public int PromotionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public PromotionType Type { get; set; } = PromotionType.Discount;
        public PromotionScope Scope { get; set; } = PromotionScope.All;
        public DiscountType DiscountType { get; set; } = DiscountType.Percentage;
        public decimal DiscountValue { get; set; }
        public int? CategoryId { get; set; }
        public int? CategoryTypeId { get; set; }
        public int? NailDesignId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = "Active";
        public string Situation { get; set; } = string.Empty;

        public bool IsSelectable { get; set; }
        public int? UsageLimit { get; set; }
        public int CurrentUsageCount { get; set; }
        public int? UserLimit { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int? PointsRequired { get; set; }
        public virtual Category? Category { get; set; }
        public virtual CategoryType? CategoryType { get; set; }
        public virtual NailDesign? NailDesign { get; set; }
        public virtual ICollection<BookingDiscount> BookingDiscounts { get; set; } = new List<BookingDiscount>();
        public virtual ICollection<UserPromotionUsage> UserPromotionUsages { get; set; } = new List<UserPromotionUsage>();

        public bool IsValid()
        {
            return Scope switch
            {
                PromotionScope.NailDesign => NailDesignId.HasValue && !CategoryId.HasValue && !CategoryTypeId.HasValue,
                PromotionScope.Category => CategoryId.HasValue && !NailDesignId.HasValue && !CategoryTypeId.HasValue,
                PromotionScope.CategoryType => CategoryTypeId.HasValue && !NailDesignId.HasValue && !CategoryId.HasValue,
                PromotionScope.All => !NailDesignId.HasValue && !CategoryId.HasValue && !CategoryTypeId.HasValue,
                PromotionScope.FirstTimeUser => !NailDesignId.HasValue && !CategoryId.HasValue && !CategoryTypeId.HasValue,
                _ => false
            };
        }
    }
}
