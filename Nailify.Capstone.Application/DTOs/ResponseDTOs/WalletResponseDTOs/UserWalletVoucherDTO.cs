using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.WalletResponseDTOs
{
    public class UserWalletVoucherDTO : IMapFrom<UserPromotionUsage>
    {
        public int UserPromotionUsageId { get; set; }
        public int PromotionId { get; set; }
        public string PromotionName { get; set; } = string.Empty;
        public string Description { get; set; }
        public string DiscountType { get; set; } = string.Empty;
        public decimal DiscountValue { get; set; }
        public int ReceivedCount { get; set; }
        public int UsageCount { get; set; }
        public int RemainingCount => Math.Max(0, ReceivedCount - UsageCount);
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsValidForUse => RemainingCount > 0 && (EndDate == null || EndDate >= DateTime.UtcNow);
        public void Mapping(Profile profile)
        {
            profile.CreateMap<UserPromotionUsage, UserWalletVoucherDTO>()
                .ForMember(d => d.PromotionName, opt => opt.MapFrom(s => s.Promotion.Name))
                .ForMember(d => d.Description, opt => opt.MapFrom(s => s.Promotion.Description))
                .ForMember(d => d.DiscountType, opt => opt.MapFrom(s => s.Promotion.DiscountType.ToString()))
                .ForMember(d => d.DiscountValue, opt => opt.MapFrom(s => s.Promotion.DiscountValue))
                .ForMember(d => d.StartDate, opt => opt.MapFrom(s => s.Promotion.StartDate))
                .ForMember(d => d.EndDate, opt => opt.MapFrom(s => s.Promotion.EndDate))
                .ForMember(d => d.ImageUrl, opt => opt.MapFrom(s => s.Promotion.ImageUrl));
        }
    }
}
