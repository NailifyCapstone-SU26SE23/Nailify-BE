using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailArtistRequestDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;

namespace Nailify.Capstone.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmergencyOffController : ControllerBase
    {
        private readonly INailArtistEmergencyService _emergencyService;
        public EmergencyOffController(INailArtistEmergencyService emergencyService)
        {
            _emergencyService = emergencyService;
        }
        /// <summary>
        /// Manager bật chế độ Tạm nghỉ Khẩn cấp (Emergency Off) cho Thợ Nail
        /// </summary>
        [HttpPost("{artistId}/emergency-off")]
        [ProducesResponseType(typeof(ApiResult<EmergencyOffResultDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SetArtistOffDuty(Guid artistId, [FromBody] EmergencyOffRequestDTO request)
        {
            var result = await _emergencyService.SetArtistOffDutyAsync(artistId, request);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
