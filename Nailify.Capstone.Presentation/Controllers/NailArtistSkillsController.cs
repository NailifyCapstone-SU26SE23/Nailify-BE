using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailArtistSkillRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.SkillTypeResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Nailify.Capstone.Presentation.Controllers
{
    [Route("api/nail-artists/{artistId}/skills")]
    [ApiController]
    public class NailArtistSkillsController : ControllerBase
    {
        private readonly INailArtistSkillService _nailArtistSkillService;
        public NailArtistSkillsController(INailArtistSkillService nailArtistSkillService)
        {
            _nailArtistSkillService = nailArtistSkillService;
        }
        /// <summary>
        /// Lấy danh sách kỹ năng của thợ nail theo ArtistId.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<List<NailArtistSkillResponseDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSkills(Guid artistId)
        {
            var response = await _nailArtistSkillService.GetSkillsByArtistIdAsync(artistId);
            if (!response.IsSucceeded) return NotFound(response);
            return Ok(response);
        }

        /// <summary>
        /// Gán hoặc cập nhật danh sách kỹ năng cho thợ nail.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<List<NailArtistSkillResponseDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AssignSkills(Guid artistId, [FromBody] List<AssignSkillRequest> requests)
        {
            var response = await _nailArtistSkillService.AssignSkillAsync(artistId, requests);
            if (!response.IsSucceeded) return BadRequest(response);
            return Ok(response);
        }

        /// <summary>
        /// Cập nhật mức độ thành thạo của một kỹ năng cho thợ nail.
        /// </summary>
        [HttpPut("{skillTypeId}")]
        [ProducesResponseType(typeof(ApiResult<NailArtistSkillResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateSkillLevel(Guid artistId, Guid skillTypeId, [FromBody] UpdateSkillLevelRequest request)
        {
            var response = await _nailArtistSkillService.UpdateSkillAsync(artistId, skillTypeId, request);
            if (!response.IsSucceeded) return BadRequest(response);
            return Ok(response);
        }

        /// <summary>
        /// Xóa kỹ năng khỏi danh sách kỹ năng của thợ nail.
        /// </summary>
        [HttpDelete("{skillTypeId}")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveSkill(Guid artistId, Guid skillTypeId)
        {
            var response = await _nailArtistSkillService.DeleteSkillAsync(artistId, skillTypeId);
            if (!response.IsSucceeded) return BadRequest(response);
            return Ok(response);
        }

    }
}
