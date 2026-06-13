using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailArtistSkillRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.SkillTypeResponseDTOs;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface INailArtistSkillService
    {
        Task<ApiResult<List<NailArtistSkillResponseDTO>>> GetSkillsByArtistIdAsync(Guid artistId);
        Task<ApiResult<List<NailArtistSkillResponseDTO>>> AssignSkillAsync(Guid artistId, List<AssignSkillRequest> requests);
        Task<ApiResult<NailArtistSkillResponseDTO>> UpdateSkillAsync(Guid artistId, Guid skillTypeId, UpdateSkillLevelRequest request);
        Task<ApiResult<bool>> DeleteSkillAsync(Guid artistId, Guid skillTypeId);
    }
}
