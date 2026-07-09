using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs
{
    public class RecommendedNailVariantResponseDTO : IMapFrom<NailVariant>
    {
        public int NailVariantId { get; set; }
        public int NailDesignId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public double Score { get; set; }
        public List<string> Reasons { get; set; } = new();
        public void Mapping(Profile profile)
        {
            profile.CreateMap<NailVariant, RecommendedNailVariantResponseDTO>()
                .ForMember(dest => dest.Score, opt => opt.Ignore())
                .ForMember(dest => dest.Reasons, opt => opt.Ignore());
        }
    }
}
