using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.SkillTypeResponseDTOs
{
    public class NailArtistSkillResponseDTO : IMapFrom<NailArtistSkill>
    {
        public Guid NailArtistSkillId { get; set; }
        public Guid NailArtistId { get; set; }
        public Guid SkillTypeId { get; set; }
        public string SkillTypeName { get; set; } = string.Empty;
        public int Level { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<NailArtistSkill, NailArtistSkillResponseDTO>()
                   .ForMember(dest => dest.SkillTypeName, opt => opt.MapFrom(x => x.SkillType.Name));
        }
    }
}
