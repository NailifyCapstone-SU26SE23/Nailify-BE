using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.CustomerNailComponentRequestDTOs
{
    public class CustomerNailComponentCreateRequest : IMapFrom<CustomerNailComponent>
    {
        public int CustomerNailId { get; set; }
        public int? ComponentId { get; set; }
        public int? CustomerComponentId { get; set; }
        public decimal PosX { get; set; }
        public decimal PosY { get; set; }
        public int FingerIndex { get; set; } = -1;
        public string ConfigJson { get; set; } = string.Empty;

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CustomerNailComponentCreateRequest, CustomerNailComponent>();
        }
    }
}
