using AutoMapper;
using Nailify.Capstone.Application.DTOs.RequestDTOs.ProcedureRequestDTOs;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Application.Mapping;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.ProcedureResponseDTOs
{
    public class ProcedureResponseDTO : IMapFrom<Procedure>
    {
        public Guid ProcedureId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? Duration { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreateAt { get; set; }
    }
}
