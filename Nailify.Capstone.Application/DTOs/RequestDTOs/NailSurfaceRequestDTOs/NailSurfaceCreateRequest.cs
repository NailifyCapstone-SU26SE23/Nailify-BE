using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.NailSurfaceRequestDTOs
{
    public class NailSurfaceCreateRequest : IMapFrom<NailSurface>
    {
        public string Name { get; set; } = string.Empty;
        public string ShaderParam { get; set; } = string.Empty;

        public float LightnessOffset { get; set; } = 0.0f;
        public float SaturationOffset { get; set; } = 0.0f; 
        public float HueOffset { get; set; } = 0.0f; 

        public decimal Price { get; set; }
        public int? Duration { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<NailSurfaceCreateRequest, NailSurface>();
        }
    }
}
