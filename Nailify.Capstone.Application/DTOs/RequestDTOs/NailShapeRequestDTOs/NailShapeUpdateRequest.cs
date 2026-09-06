using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.NailShapeRequestDTOs
{
    public class NailShapeUpdateRequest : IMapFrom<NailShape>
    {
        public string Name { get; set; } = string.Empty;

        public void Mapping(Profile profile)
        {
            profile.CreateMap<NailShapeUpdateRequest, NailShape>()
                .ForMember(dest => dest.NailShapeId, opt => opt.Ignore());
        }
    }
}
