using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.SkillTypeResponseDTOs
{
    public class NailRequiredSkillResponseDTO : IMapFrom<NailRequiredSkill>
    {
        public Guid NailRequiredSkillId { get; set; }
        public int NailVariantId { get; set; }
        public Guid SkillTypeId { get; set; }
        public string SkillTypeName { get; set; } = string.Empty;
        public int RequiredLevel { get; set; }

        public void Mapping(AutoMapper.Profile profile)
        {
            profile.CreateMap<NailRequiredSkill, NailRequiredSkillResponseDTO>()
                   .ForMember(dest => dest.SkillTypeName, opt => opt.MapFrom(x => x.SkillType.Name));
        }
    }
}
