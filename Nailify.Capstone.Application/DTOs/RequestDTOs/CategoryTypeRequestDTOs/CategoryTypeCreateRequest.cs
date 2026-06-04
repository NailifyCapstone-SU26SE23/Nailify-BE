using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.CategoryTypeRequestDTOs
{
    public class CategoryTypeCreateRequest : IMapFrom<CategoryType>
    {
        public string Name { get; set; } = string.Empty;

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CategoryTypeCreateRequest, CategoryType>()
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.Categories, opt => opt.Ignore());
        }
    }
}
