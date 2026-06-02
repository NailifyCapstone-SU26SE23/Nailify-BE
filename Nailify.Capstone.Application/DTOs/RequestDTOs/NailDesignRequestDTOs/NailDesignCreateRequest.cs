using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.NailDesignRequestDTOs
{
    public class NailDesignCreateRequest : IMapFrom<NailDesign>
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<int> CategoryIds { get; set; } = new List<int>();
        public List<int> NailVariantIds { get; set; } = new List<int>();

        public void Mapping(Profile profile)
        {
            profile.CreateMap<NailDesignCreateRequest, NailDesign>()
                .ForMember(dest => dest.NailCategories, opt => opt.Ignore())
                .ForMember(dest => dest.NailDesignImages, opt => opt.Ignore())
                .ForMember(dest => dest.NailVariants, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore());
        }
    }
}
