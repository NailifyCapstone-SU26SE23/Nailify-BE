using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.CategoryRequestDTOs
{
    public class CategoryCreateRequest : IMapFrom<Category>
    {
        public string Name { get; set; } = string.Empty;
        public int CategoryTypeId { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CategoryCreateRequest, Category>()
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.CategoryType, opt => opt.Ignore())
                .ForMember(dest => dest.NailCategories, opt => opt.Ignore());
        }
    }
}
