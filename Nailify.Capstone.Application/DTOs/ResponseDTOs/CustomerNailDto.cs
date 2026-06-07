using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs
{
    public class CustomerNailDto : IMapFrom<CustomerNail>
    {
        public int CustomerNailId { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int NailShapeId { get; set; }
        public int NailSurfaceId { get; set; }
        public decimal Price { get; set; }
        public string CustomColor { get; set; } = string.Empty;
        public string CustomMaterial { get; set; } = string.Empty;
        public int? Duration { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsFavorite { get; set; }
        public bool IsPublic { get; set; }
        public int? BasedOnNailVariantId { get; set; }
        public NailShapeDto? NailShape { get; set; }
        public NailSurfaceDto? NailSurface { get; set; }
        public NailVariantDto? BasedOnNailVariant { get; set; }
        public List<CustomerNailComponentDto> CustomerNailComponents { get; set; } = new List<CustomerNailComponentDto>();

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CustomerNail, CustomerNailDto>();
        }
    }
}
