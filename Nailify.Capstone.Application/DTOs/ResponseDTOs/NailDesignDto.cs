using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs
{
    public class NailDesignDto : IMapFrom<NailDesign>
    {
        public int NailDesignId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public List<string> ImageUrls { get; set; } = new List<string>();
        public List<CategoryDto> Categories { get; set; } = new List<CategoryDto>();

        public void Mapping(Profile profile)
        {
            profile.CreateMap<NailDesign, NailDesignDto>()
                .ForMember(dest => dest.ImageUrls,
                    opt => opt.MapFrom(src => src.NailDesignImages.Select(image => image.ImageUrl)))
                .ForMember(dest => dest.Categories,
                    opt => opt.MapFrom(src => src.NailCategories.Select(nc => nc.Category)));
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
