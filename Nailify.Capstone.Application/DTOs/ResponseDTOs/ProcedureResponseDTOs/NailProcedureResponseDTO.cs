using System;
using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.ProcedureResponseDTOs
{
    public class NailProcedureResponseDTO : IMapFrom<NailProcedure>
    {
        public Guid NailProcedureId { get; set; }
        public int? NailVariantId { get; set; }
        public int? CustomerNailId { get; set; }
        public Guid? ProcedureId { get; set; }
        public string? Name { get; set; }
        public int? EstimatedMinutes { get; set; }
        public decimal? Price { get; set; }
        public string? Note { get; set; }
        public bool IsCustomStep { get; set; }
        public string ProcedureName { get; set; } = string.Empty;
        public string? ProcedureDescription { get; set; }
        public int? ProcedureDuration { get; set; }
        public int StepOrder { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ProcedureType { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<NailProcedure, NailProcedureResponseDTO>()
                   .ForMember(dest => dest.ProcedureName, opt => opt.MapFrom(src => src.Procedure != null ? src.Procedure.Name : string.Empty))
                   .ForMember(dest => dest.ProcedureDescription, opt => opt.MapFrom(src => src.Procedure != null ? src.Procedure.Description : (string?)null))
                   .ForMember(dest => dest.ProcedureDuration, opt => opt.MapFrom(src => src.Procedure != null ? src.Procedure.ActiveDuration : (int?)null))
                   .ForMember(dest => dest.ProcedureType, opt => opt.MapFrom(src => src.Procedure != null ? src.Procedure.ProcedureType.ToString() : (string?)null));
        }
    }
}
