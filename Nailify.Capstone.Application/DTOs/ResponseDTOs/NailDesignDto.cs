using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs
{
    public class NailDesignDto : IMapFrom<NailDesign>
    {
        public int NailDesignId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsFavorited { get; set; }
        public int? FavoriteNailId { get; set; }
        public List<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
        public List<NailVariantDto> NailVariants { get; set; } = new List<NailVariantDto>();

        public void Mapping(Profile profile)
        {
            profile.CreateMap<NailDesign, NailDesignDto>()
                .ForMember(dest => dest.Categories,
                    opt => opt.MapFrom(src => src.NailCategories.Select(nc => nc.Category)))
                .ForMember(dest => dest.NailVariants,
                    opt => opt.MapFrom(src => src.NailVariants));
        }
    }

    public class CategoryDto : IMapFrom<Category>
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CategoryTypeId { get; set; }
        public string CategoryTypeName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.CategoryTypeName,
                    opt => opt.MapFrom(src => src.CategoryType.Name));
        }
    }
}
