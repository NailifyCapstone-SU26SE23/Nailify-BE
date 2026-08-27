using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Domain.Entities
{
    public class Customer
    {
        public Guid UserId { get; set; } //  PK và FK sang bảng User
        public int LoyaltyPoint { get; set; } = 0;
        public int LifetimePoints { get; set; } = 0;

        public string? SkinTone { get; set; } = string.Empty;
        public string? SkinShade { get; set; } = string.Empty;
        public string? HandShape { get; set; } = string.Empty;
        public string? Occupation { get; set; } = string.Empty;
        public string? NailCondition { get; set; } = string.Empty;
        public int? LoyaltyTierId { get; set; }

        public string PreferredColorsJson { get; set; } = string.Empty;
        public string PreferredStylesJson { get; set; } = string.Empty;
        public string PreferredOccasionsJson { get; set; } = string.Empty;
        public int? PreferredNailShapeId { get; set; }
        public string PreferredComplexity { get; set; } = string.Empty;

        public virtual User User { get; set; } = null!;
        public virtual LoyaltyTier? LoyaltyTier { get; set; }
        public virtual ICollection<LoyaltyTransaction> LoyaltyTransactions { get; set; } = new List<LoyaltyTransaction>();
        public virtual ICollection<BookingRating> BookingRatings { get; set; } = new List<BookingRating>();
        public virtual ICollection<CustomerQuizAnswer> CustomerQuizAnswers { get; set; } = new List<CustomerQuizAnswer>();
    }
}
