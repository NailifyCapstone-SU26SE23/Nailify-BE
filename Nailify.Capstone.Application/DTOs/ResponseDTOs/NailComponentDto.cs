using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs
{
    public class NailComponentDto : IMapFrom<NailComponent>
    {
        public int NailComponentId { get; set; }
        public int ComponentId { get; set; }
        public int NailVariantId { get; set; }
        public decimal PosX { get; set; }
        public decimal PosY { get; set; }
        public int FingerIndex { get; set; }
        public string ConfigJson { get; set; } = string.Empty;
        public ComponentDto? Component { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<NailComponent, NailComponentDto>();
        }
    }
}
