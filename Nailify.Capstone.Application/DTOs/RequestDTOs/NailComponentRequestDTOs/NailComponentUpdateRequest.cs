using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.NailComponentRequestDTOs
{
    public class NailComponentUpdateRequest : IMapFrom<NailComponent>
    {
        public int NailComponentId { get; set; }
        public int ComponentId { get; set; }
        public int NailVariantId { get; set; }
        public decimal PosX { get; set; }
        public decimal PosY { get; set; }
        public int FingerIndex { get; set; } = -1;
        public string ConfigJson { get; set; } = string.Empty;

        public void Mapping(Profile profile)
        {
            profile.CreateMap<NailComponentUpdateRequest, NailComponent>();
        }
    }
}
