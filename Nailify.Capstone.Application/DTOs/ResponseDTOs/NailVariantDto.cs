using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs
{
    public class NailVariantDto : IMapFrom<NailVariant>
    {
        public int NailVariantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int NailShapeId { get; set; }
        public int NailSurfaceId { get; set; }
        public int NailDesignId { get; set; }
        public decimal Price { get; set; }
        public int? Duration { get; set; }
        public decimal? Precision { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Form { get; set; } = string.Empty;
        public string Material { get; set; } = string.Empty;
        public int? Speed { get; set; }
        public NailShapeDto? NailShape { get; set; }
        public NailSurfaceDto? NailSurface { get; set; }
        public List<NailComponentDto> NailComponents { get; set; } = new List<NailComponentDto>();

        public void Mapping(Profile profile)
        {
            profile.CreateMap<NailVariant, NailVariantDto>();
        }
    }
}
