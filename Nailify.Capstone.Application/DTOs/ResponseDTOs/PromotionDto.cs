using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs
{
    public class PromotionDto : IMapFrom<Promotion>
    {
        public int PromotionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public PromotionType Type { get; set; }
        public PromotionScope Scope { get; set; }
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public int? CategoryId { get; set; }
        public int? CategoryTypeId { get; set; }
        public int? NailDesignId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int? UsageLimit { get; set; }
        public int CurrentUsageCount { get; set; }
        public int? UserLimit { get; set; }
        public string ImageUrl { get; set; } = string.Empty;

        public void Mapping(Profile profile) => profile.CreateMap<Promotion, PromotionDto>();
    }
}
