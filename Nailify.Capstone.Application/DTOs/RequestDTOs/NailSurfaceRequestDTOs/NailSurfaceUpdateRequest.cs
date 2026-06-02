using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.NailSurfaceRequestDTOs
{
    public class NailSurfaceUpdateRequest : IMapFrom<NailSurface>
    {
        public int NailSurfaceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ShaderParam { get; set; } = string.Empty;
        public decimal Price { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<NailSurfaceUpdateRequest, NailSurface>();
        }
    }
}
