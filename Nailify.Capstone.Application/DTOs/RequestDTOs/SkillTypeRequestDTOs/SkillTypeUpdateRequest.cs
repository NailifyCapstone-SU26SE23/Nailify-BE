using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Application.Mapping;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.SkillTypeRequestDTOs
{
    public class SkillTypeUpdateRequest : IMapFrom<SkillType>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ActiveStatusFilter? Status { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<SkillTypeUpdateRequest, SkillType>()
                   .IgnoreAllNonExisting();
        }
    }
}