using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.NailShapeRequestDTOs
{
    public class NailShapeUpdateRequest : IMapFrom<NailShape>
    {
        public int NailShapeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public decimal Price { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<NailShapeUpdateRequest, NailShape>();
        }
    }
}
