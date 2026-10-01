using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.NailVariantRequestDTOs
{
    public class NailVariantUpdateRequest : IMapFrom<NailVariant>
    {
        public string Name { get; set; } = string.Empty;
        public int? NailShapeId { get; set; }
        public int? NailSurfaceId { get; set; }
        public int? NailDesignId { get; set; }
        public string ColorJson { get; set; } = string.Empty;
        public ActiveStatusFilter? Status { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<NailVariantUpdateRequest, NailVariant>()
                .ForMember(dest => dest.NailVariantId, opt => opt.Ignore())
                .ForMember(dest => dest.Price, opt => opt.Ignore())
                .ForMember(dest => dest.Duration, opt => opt.Ignore())
                .ForMember(dest => dest.NailShapeId, opt => opt.Condition(src => src.NailShapeId.HasValue))
                .ForMember(dest => dest.NailSurfaceId, opt => opt.Condition(src => src.NailSurfaceId.HasValue))
                .ForMember(dest => dest.NailDesignId, opt => opt.Condition(src => src.NailDesignId.HasValue));
        }
    }
}
