using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;
using System;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs
{
    public class CustomerProfileDto : IMapFrom<User>, IMapFrom<Customer>
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; } = string.Empty;
        public string? FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        // Thông tin đặc thù mở rộng từ bảng Customer
        public int LoyaltyPoint { get; set; }
        public int LifetimePoints { get; set; }
        public string? SkinTone { get; set; } = string.Empty;
        public string? Occupation { get; set; } = string.Empty;
        public string? NailCondition { get; set; } = string.Empty;
        public string? PersonaId { get; set; } = string.Empty;
        public string? SkinShade { get; set; } = string.Empty;
        public string? HandShape { get; set; } = string.Empty;
        public string? PreferredComplexity { get; set; } = string.Empty;

        public List<string> PreferredColors { get; set; } = new();
        public List<string> PreferredStyles { get; set; } = new();
        public List<string> PreferredOccasions { get; set; } = new();

        public int? PreferredNailShapeId { get; set; }
        public string PreferredNailShapeName { get; set; } = string.Empty;
        // mapping
        public void Mapping(Profile profile)
        {
            profile.CreateMap<User, CustomerProfileDto>();
            profile.CreateMap<Customer, CustomerProfileDto>();
        }
    }
}
