using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.CategoryTypeRequestDTOs
{
    public class CategoryTypeUpdateRequest : IMapFrom<CategoryType>
    {
        public int CategoryTypeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CategoryTypeUpdateRequest, CategoryType>()
                .ForMember(dest => dest.Categories, opt => opt.Ignore());
        }
    }
}
