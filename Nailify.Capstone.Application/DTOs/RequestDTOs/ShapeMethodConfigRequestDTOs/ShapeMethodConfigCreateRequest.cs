using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Application.Mapping;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.ShapeMethodConfigRequestDTOs
{
    public class ShapeMethodConfigCreateRequest : IMapFrom<ShapeMethodConfig>
    {
        public int NailShapeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Duration { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ShapeMethodConfigCreateRequest, ShapeMethodConfig>()
                .IgnoreAllNonExisting();
        }
    }
}
