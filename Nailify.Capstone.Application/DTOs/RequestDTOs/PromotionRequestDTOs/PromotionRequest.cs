using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.PromotionRequestDTOs
{
    public class PromotionRequest : IMapFrom<Promotion>
    {
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
        public bool IsSelectable { get; set; }
        public int? UsageLimit { get; set; }
        public int? UserLimit { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<PromotionRequest, Promotion>();
        }
    }
}
