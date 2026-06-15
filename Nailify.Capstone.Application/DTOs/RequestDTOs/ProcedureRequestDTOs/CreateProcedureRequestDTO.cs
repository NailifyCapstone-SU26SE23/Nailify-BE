using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Application.Mapping;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.ProcedureRequestDTOs
{
    public class CreateProcedureRequestDTO : IMapFrom<Procedure>
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? Duration { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<CreateProcedureRequestDTO, Procedure>()
                   .IgnoreAllNonExisting();
        }
    }
}
