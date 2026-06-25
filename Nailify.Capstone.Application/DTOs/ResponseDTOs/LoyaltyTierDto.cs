using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs
{
    public class LoyaltyTierDto : IMapFrom<LoyaltyTier>
    {
        public int LoyaltyTierId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int? MinLifetimePoints { get; set; }
        public int? MaxLifetimePoints { get; set; }
        public decimal DiscountRate { get; set; }
        public string? ImageUrl { get; set; }
        public string? BackgroundColor { get; set; }
        public string? TextColor { get; set; }
        public string? ColorJson { get; set; }
        public string Status { get; set; } = string.Empty;
        public int? SortOrder { get; set; }

        public void Mapping(Profile profile) => profile.CreateMap<LoyaltyTier, LoyaltyTierDto>();
    }
}
