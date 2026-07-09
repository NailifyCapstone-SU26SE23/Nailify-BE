using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.NailArtistBreakRequestDTOs
{
    public class NailArtistBreakUpdateRequestDTO : IMapFrom<NailArtistBreak>
    {
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<NailArtistBreakUpdateRequestDTO, NailArtistBreak>()
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => TimeSpan.Parse(src.StartTime)))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => TimeSpan.Parse(src.EndTime)));
        }
    }
}
