using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Domain.Enums;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.ProcedureRequestDTOs
{
    public class ProcedurePagingParameters : PagingRequestParameters
    {
        /// <summary>
        /// Lọc theo loại quy trình (Common = 1: Quy trình chung, ModelSpecific = 2: Quy trình riêng theo mẫu)
        /// </summary>
        public ProcedureType? ProcedureType { get; set; }
    }
}
