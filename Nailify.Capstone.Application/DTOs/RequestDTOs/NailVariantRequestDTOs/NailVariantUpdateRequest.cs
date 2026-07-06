using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.NailVariantRequestDTOs
{
    public class NailVariantUpdateRequest : IMapFrom<NailVariant>
    {
        public string Name { get; set; } = string.Empty;
        public int? NailShapeId { get; set; }
        public int? NailSurfaceId { get; set; }
        public int? NailDesignId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;        
        public string ColorJson { get; set; } = string.Empty;

        public void Mapping(Profile profile)
        {
            profile.CreateMap<NailVariantUpdateRequest, NailVariant>()
                .ForMember(dest => dest.NailVariantId, opt => opt.Ignore())
                .ForMember(dest => dest.Duration, opt => opt.Ignore());
        }
    }
}
