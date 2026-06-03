using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.CustomerNailRequestDTOs
{
    public class CustomerNailUpdateRequest : IMapFrom<CustomerNail>
    {
        public int CustomerNailId { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int NailShapeId { get; set; }
        public int NailSurfaceId { get; set; }
        public string CustomColor { get; set; } = string.Empty;
        public string CustomMaterial { get; set; } = string.Empty;
        public int? Duration { get; set; }
        public bool IsFavorite { get; set; }
        public bool IsPublic { get; set; }
        public int? BasedOnNailVariantId { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CustomerNailUpdateRequest, CustomerNail>()
                .ForMember(dest => dest.CustomerNailComponents, opt => opt.Ignore())
                .ForMember(dest => dest.Price, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        }
    }
}
