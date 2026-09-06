using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs
{
    public class NailSurfaceDto : IMapFrom<NailSurface>
    {
        public int NailSurfaceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ShaderParam { get; set; } = string.Empty;
        public float LightnessOffset { get; set; } = 0.0f;
        public float SaturationOffset { get; set; } = 0.0f;
        public float HueOffset { get; set; } = 0.0f;
        public decimal Price { get; set; }
        public int? Duration { get; set; }
    }
}
