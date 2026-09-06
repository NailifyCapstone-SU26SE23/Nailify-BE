using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs
{
    public class FavoriteNailDto : IMapFrom<FavoriteNail>
    {
        public int FavoriteNailId { get; set; }
        public int? NailDesignId { get; set; }
        public int? NailVariantId { get; set; }
        public DateTime CreatedAt { get; set; }
        public NailDesignDto? NailDesign { get; set; }
        public NailVariantDto? NailVariant { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<FavoriteNail, FavoriteNailDto>();
        }
    }
}
