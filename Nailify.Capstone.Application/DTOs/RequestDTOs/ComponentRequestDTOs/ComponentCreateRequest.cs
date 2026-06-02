using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.ComponentRequestDTOs
{
    public class ComponentCreateRequest : IMapFrom<Component>
    {
        public string Name { get; set; } = string.Empty;
        public ComponentType ComponentType { get; set; }
        public decimal Price { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ComponentCreateRequest, Component>();
        }
    }
}
