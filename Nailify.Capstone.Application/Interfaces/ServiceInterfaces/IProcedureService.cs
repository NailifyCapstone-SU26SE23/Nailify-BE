using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.ProcedureRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.ProcedureResponseDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface IProcedureService
    {
        Task<ApiResult<PagedList<ProcedureResponseDTO>>> GetAllProceduresAsync(PagingRequestParameters parameters);
        Task<ApiResult<ProcedureResponseDTO>> GetProcedureByIdAsync(Guid procedureId);
        Task<ApiResult<ProcedureResponseDTO>> CreateProcedureAsync(CreateProcedureRequestDTO request);
        Task<ApiResult<ProcedureResponseDTO>> UpdateProcedureAsync(Guid procedureId, UpdateProcedureRequestDTO request);
        Task<ApiResult<bool>> DeleteProcedureAsync(Guid procedureId); 
        Task<ApiResult<List<ProcedureResponseDTO>>> GetProceduresByVariantIdAsync(int nailVariantId);
        Task<ApiResult<bool>> AssignProceduresToVariantAsync(int nailVariantId, List<AssignProcedureRequestDTO> request);
    }
}
