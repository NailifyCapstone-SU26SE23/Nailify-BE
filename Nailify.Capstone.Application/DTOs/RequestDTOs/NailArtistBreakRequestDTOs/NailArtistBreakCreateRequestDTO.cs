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
    public class NailArtistBreakCreateRequestDTO : IMapFrom<NailArtistBreak>
    {
        public Guid NailArtistId { get; set; }
        public DateTime BreakDate { get; set; }
        public string StartTime { get; set; } = string.Empty; // e.g. "15:00"
        public string EndTime { get; set; } = string.Empty;   // e.g. "16:00"
        public string? Reason { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<NailArtistBreakCreateRequestDTO, NailArtistBreak>()
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => TimeSpan.Parse(src.StartTime)))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => TimeSpan.Parse(src.EndTime)));
        }
    }
}
