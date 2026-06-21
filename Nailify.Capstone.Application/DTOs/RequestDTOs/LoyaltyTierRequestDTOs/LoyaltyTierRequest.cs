using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.LoyaltyTierRequestDTOs
{
    public class LoyaltyTierRequest : IMapFrom<LoyaltyTier>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int? MinLifetimePoints { get; set; }
        public int? MaxLifetimePoints { get; set; }
        public decimal DiscountRate { get; set; }
        public string? BackgroundColor { get; set; }
        public string? TextColor { get; set; }
        public string? ColorJson { get; set; }
        public int? SortOrder { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<LoyaltyTierRequest, LoyaltyTier>();
        }
    }
}
