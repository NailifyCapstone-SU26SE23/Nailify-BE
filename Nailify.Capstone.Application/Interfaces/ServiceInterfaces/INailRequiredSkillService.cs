using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailRequiredSkillRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.SkillTypeResponseDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface INailRequiredSkillService
    {
        Task<ApiResult<List<NailRequiredSkillResponseDTO>>> GetRequiredSkillsByDesignIdAsync(int nailId);
        Task<ApiResult<List<NailRequiredSkillResponseDTO>>> AssignRequiredSkillsAsync(int designId, List<AssignRequiredSkillRequest> requests);
        Task<ApiResult<NailRequiredSkillResponseDTO>> UpdateRequiredSkillLevelAsync(int designId, Guid skillId, UpdateRequiredSkillLevelRequest request);
        Task<ApiResult<bool>> DeleteRequiredSkillAsync(int designId, Guid skillId);
    }
}
