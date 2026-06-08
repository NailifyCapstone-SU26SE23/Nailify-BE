using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.CustomerComponentRequestDTOs
{
    public class CustomerComponentCreateRequest : IMapFrom<CustomerComponent>
    {
        public string Name { get; set; } = string.Empty;
        public ComponentType ComponentType { get; set; }
        public decimal? Price { get; set; }
        public bool IsPublic { get; set; } = false;

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CustomerComponentCreateRequest, CustomerComponent>()
                .ForMember(dest => dest.CustomerNailComponents, opt => opt.Ignore())
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        }
    }
}
