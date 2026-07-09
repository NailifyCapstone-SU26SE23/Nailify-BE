using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.NailArtistBreakResponseDTOs
{
    public class NailArtistBreakResponseDTO : IMapFrom<NailArtistBreak>
    {
        public Guid NailArtistBreakId { get; set; }
        public Guid NailArtistId { get; set; }
        public DateTime BreakDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string? Reason { get; set; }
        public string Status { get; set; } = string.Empty;
        public void Mapping(Profile profile)
        {
            profile.CreateMap<NailArtistBreak, NailArtistBreakResponseDTO>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        }
    }
}
