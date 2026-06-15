using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.ProcedureRequestDTOs
{
    public class AssignProcedureRequestDTO
    {
        public Guid ProcedureId { get; set; }
        public int StepOrder { get; set; }
    }
}
