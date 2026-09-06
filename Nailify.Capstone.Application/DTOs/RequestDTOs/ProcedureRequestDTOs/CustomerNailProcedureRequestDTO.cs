using System;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.ProcedureRequestDTOs
{
    public class CustomerNailProcedureRequestDTO
    {
        public Guid ProcedureId { get; set; }
        public int StepOrder { get; set; }
    }
}
