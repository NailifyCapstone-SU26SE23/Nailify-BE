using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingResponseDTOs
{
    public class SuggestedArtistResponseDTO : IMapFrom<NailArtist>
    {
        public Guid NailArtistId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public List<ArtistSkillInfoResponseDto> Skills { get; set; } = new();
        public void Mapping(Profile profile)
        {
            profile.CreateMap<NailArtist, SuggestedArtistResponseDTO>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Account.FirstName + " " + src.Account.LastName))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.Account.AvatarUrl ?? ""))
                .ForMember(dest => dest.Skills, opt => opt.MapFrom(src => src.NailArtistSkills));
        }
    }
}
