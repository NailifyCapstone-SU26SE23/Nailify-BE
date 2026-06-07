using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.CategoryRequestDTOs
{
    public class CategoryUpdateRequest : IMapFrom<Category>
    {
        public string Name { get; set; } = string.Empty;
        public int CategoryTypeId { get; set; }
        public string Status { get; set; } = "Active";

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CategoryUpdateRequest, Category>()
                .ForMember(dest => dest.CategoryId, opt => opt.Ignore())
                .ForMember(dest => dest.CategoryType, opt => opt.Ignore())
                .ForMember(dest => dest.NailCategories, opt => opt.Ignore());
        }
    }
}
