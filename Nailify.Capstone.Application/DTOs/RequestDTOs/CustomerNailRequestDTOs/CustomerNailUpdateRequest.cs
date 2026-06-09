using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.CustomerNailRequestDTOs
{
    public class CustomerNailUpdateRequest : IMapFrom<CustomerNail>
    {
        public string Name { get; set; } = string.Empty;
        public int? NailShapeId { get; set; }
        public int? NailSurfaceId { get; set; }
        public string? CustomColor { get; set; } 
        public bool IsFavorite { get; set; }
        public bool IsPublic { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CustomerNailUpdateRequest, CustomerNail>()
                .ForMember(dest => dest.CustomerNailId, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerNailComponents, opt => opt.Ignore())
                .ForMember(dest => dest.Price, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        }
    }
}
