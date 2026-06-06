using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.NailVariantRequestDTOs
{
    public class NailVariantUpdateRequest : IMapFrom<NailVariant>
    {
        public int NailVariantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? NailShapeId { get; set; }
        public int? NailSurfaceId { get; set; }
        public int NailDesignId { get; set; }
        public int? Duration { get; set; }
        public string ImageUrl { get; set; } = string.Empty;        
        public string ColorJson { get; set; } = string.Empty;

        public void Mapping(Profile profile)
        {
            profile.CreateMap<NailVariantUpdateRequest, NailVariant>();
        }
    }
}
