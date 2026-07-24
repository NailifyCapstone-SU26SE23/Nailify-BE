using System;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.ProcedureResponseDTOs
{
    public class NailProcedureResponseDTO
    {
        public Guid NailProcedureId { get; set; }
        public int? NailVariantId { get; set; }
        public int? CustomerNailId { get; set; }
        public Guid ProcedureId { get; set; }
        public string ProcedureName { get; set; } = string.Empty;
        public string? ProcedureDescription { get; set; }
        public int? ProcedureDuration { get; set; }
        public int StepOrder { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
