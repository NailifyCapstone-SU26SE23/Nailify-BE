using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs
{
    public class NailCategoryDto : IMapFrom<NailCategory>
    {
        public int NailCategoryId { get; set; }
        public int NailDesignId { get; set; }
        public int CategoryId { get; set; }
        public CategoryDto? Category { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<NailCategory, NailCategoryDto>();
        }
    }
}
