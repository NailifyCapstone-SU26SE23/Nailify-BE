using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Application.Mapping;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.SkillTypeRequestDTOs
{
    public class SkillTypeCreateRequest : IMapFrom<SkillType>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<SkillTypeCreateRequest, SkillType>()
                   .IgnoreAllNonExisting();
        }
    }
}
