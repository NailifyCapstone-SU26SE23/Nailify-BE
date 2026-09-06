using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.CustomerComponentRequestDTOs
{
    public class CustomerComponentUpdateRequest : IMapFrom<CustomerComponent>
    {
        public string Name { get; set; } = string.Empty;
        public ComponentType ComponentType { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CustomerComponentUpdateRequest, CustomerComponent>()
                .ForMember(dest => dest.CustomerComponentId, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerNailComponents, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        }
    }
}
