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
    public class WalletSummaryDTO : IMapFrom<Customer>
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? AvatarUrl { get; set; }
        public int LoyaltyPoint { get; set; }
        public int LifetimePoints { get; set; }
        public int? CurrentLoyaltyTierId { get; set; }
        public string TierName { get; set; } = string.Empty;
        public decimal DiscountRate { get; set; }
        public int AvailableVouchersCount { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<Customer, WalletSummaryDTO>()
                .ForMember(d => d.Email, opt => opt.MapFrom(s => s.User.Email))
                .ForMember(d => d.Phone, opt => opt.MapFrom(s => s.User.Phone))
                .ForMember(d => d.FirstName, opt => opt.MapFrom(s => s.User.FirstName))
                .ForMember(d => d.LastName, opt => opt.MapFrom(s => s.User.LastName))
                .ForMember(d => d.AvatarUrl, opt => opt.MapFrom(s => s.User.AvatarUrl))
                .ForMember(d => d.CurrentLoyaltyTierId, opt => opt.MapFrom(s => s.LoyaltyTierId))
                .ForMember(d => d.TierName, opt => opt.MapFrom(s => s.LoyaltyTier != null ? s.LoyaltyTier.Name : string.Empty))
                .ForMember(d => d.DiscountRate, opt => opt.MapFrom(s => s.LoyaltyTier != null ? s.LoyaltyTier.DiscountRate : 0m));
        }
    }
}
