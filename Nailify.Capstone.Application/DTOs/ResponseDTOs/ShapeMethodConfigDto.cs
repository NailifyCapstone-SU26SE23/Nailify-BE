using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs
{
    public class ShapeMethodConfigDto : IMapFrom<ShapeMethodConfig>
    {
        public int ShapeMethodConfigId { get; set; }
        public int NailShapeId { get; set; }
        public string NailShapeName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public string Status { get; set; } = string.Empty;

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ShapeMethodConfig, ShapeMethodConfigDto>()
                .ForMember(dest => dest.NailShapeName, opt => opt.MapFrom(src => src.NailShape != null ? src.NailShape.Name : ""));
        }
    }
}
