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
    public class ArtistSkillInfoResponseDto : IMapFrom<NailArtistSkill>
    {
        public string SkillTypeName { get; set; } = string.Empty;
        public int Level { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<NailArtistSkill, ArtistSkillInfoResponseDto>()
                .ForMember(dest => dest.SkillTypeName, opt => opt.MapFrom(src => src.SkillType.Name));
        }
    }
}
