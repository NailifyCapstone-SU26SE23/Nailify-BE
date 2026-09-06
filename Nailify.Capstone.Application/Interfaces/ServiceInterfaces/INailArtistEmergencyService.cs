using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailArtistRequestDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface INailArtistEmergencyService
    {
        Task<ApiResult<EmergencyOffResultDTO>> SetArtistOffDutyAsync(Guid artistId, EmergencyOffRequestDTO request);
        Task<EmergencyOffResultDTO> ProcessAffectedBookingsForDateAsync(Guid artistId, DateTime targetDate, string reason);
    }
}
