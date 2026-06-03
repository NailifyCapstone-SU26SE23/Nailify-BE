using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.CustomerComponentRequestDTOs
{
    public class CustomerComponentUpdateRequest : IMapFrom<CustomerComponent>
    {
        public int CustomerComponentId { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public ComponentType ComponentType { get; set; }
        public decimal? Price { get; set; }
        public string CustomDataJson { get; set; } = string.Empty;
        public bool IsPublic { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CustomerComponentUpdateRequest, CustomerComponent>()
                .ForMember(dest => dest.CustomerNailComponents, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        }
    }
}
