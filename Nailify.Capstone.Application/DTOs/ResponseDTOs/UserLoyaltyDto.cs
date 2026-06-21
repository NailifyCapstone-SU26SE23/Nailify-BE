namespace Nailify.Capstone.Application.DTOs.ResponseDTOs
{
    public class UserLoyaltyDto
    {
        public int LoyaltyPoint { get; set; }
        public int LifetimePoints { get; set; }
        public LoyaltyTierDto LoyaltyTier { get; set; } = null!;
    }
}
