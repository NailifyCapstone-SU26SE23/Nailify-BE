using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs
{
    public class NailVariantDto : IMapFrom<NailVariant>
    {
        public int NailVariantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? NailShapeId { get; set; }
        public int? NailSurfaceId { get; set; }
        public int? NailDesignId { get; set; }
        public decimal Price { get; set; }
        public decimal EstimatedPrice { get; set; }
        public int? Duration { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string ColorJson { get; set; } = string.Empty;
        public bool IsFavorited { get; set; }
        public int? FavoriteNailId { get; set; }
        public NailShapeDto? NailShape { get; set; }
        public NailSurfaceDto? NailSurface { get; set; }
        public List<NailComponentDto> NailComponents { get; set; } = new List<NailComponentDto>();

        public void Mapping(Profile profile)
        {
            profile.CreateMap<NailVariant, NailVariantDto>()
                .ForMember(dest => dest.EstimatedPrice,
                    opt => opt.MapFrom(src =>
                        src.Price + (src.NailShape != null
                            ? src.NailShape.ShapeMethodConfigs
                            .Where(config => config.Status == "Active")
                            .Select(config => (decimal?)config.Price)
                            .Min() ?? 0m
                            : 0m)));
        }
    }
}
