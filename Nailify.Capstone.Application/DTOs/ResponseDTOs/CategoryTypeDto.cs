using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs
{
    public class CategoryTypeDto : IMapFrom<CategoryType>
    {
        public int CategoryTypeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public List<CategoryDto> Categories { get; set; } = new List<CategoryDto>();

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CategoryType, CategoryTypeDto>();
        }
    }
}
