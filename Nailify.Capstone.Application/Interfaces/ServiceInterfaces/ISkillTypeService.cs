using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.SkillTypeRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.SkillTypeResponseDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface ISkillTypeService
    {
        Task<ApiResult<PagedList<SkillTypeResponseDTO>>> GetPagedSkillTypesAsync(int pageNumber, int pageSize, string? name = null, string? status = null,
          string? orderBy = null);
        Task<ApiResult<SkillTypeResponseDTO>> GetSkillTypeByIdAsync(Guid skillTypeId);
         Task<ApiResult<SkillTypeResponseDTO>> CreateSkillTypeAsync(SkillTypeCreateRequest request);
        Task<ApiResult<SkillTypeResponseDTO>> UpdateSkillTypeAsync(Guid skillTypeId, SkillTypeUpdateRequest request);
        Task<ApiResult<bool>> DeleteSkillTYpeAsync(Guid skillTypeId);
    }
}
