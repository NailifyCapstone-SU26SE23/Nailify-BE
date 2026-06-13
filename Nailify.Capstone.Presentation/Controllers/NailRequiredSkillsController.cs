using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailRequiredSkillRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.SkillTypeResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Nailify.Capstone.Presentation.Controllers
{
    [Route("api/nail-designs/{variantId}/required-skills")]
    [ApiController]
    public class NailRequiredSkillsController : ControllerBase
    {
        private readonly INailRequiredSkillService _nailRequiredSkillService;
        public NailRequiredSkillsController(INailRequiredSkillService nailRequiredSkillService)
        {
            _nailRequiredSkillService = nailRequiredSkillService;
        }
        /// <summary>
        /// Lấy danh sách kỹ năng yêu cầu của thiết kế nail theo DesignId.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<List<NailRequiredSkillResponseDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRequiredSkills(int variantId)
        {
            var result = await _nailRequiredSkillService.GetRequiredSkillsByDesignIdAsync(variantId);
            if (!result.IsSucceeded) return NotFound(result);
            return Ok(result);
        }

        /// <summary>
        /// Gán hoặc cập nhật danh sách kỹ năng yêu cầu cho thiết kế nail.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<List<NailRequiredSkillResponseDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AssignRequiredSkills(int variantId, [FromBody] List<AssignRequiredSkillRequest> requests)
        {
            var result = await _nailRequiredSkillService.AssignRequiredSkillsAsync(variantId, requests);
            if (!result.IsSucceeded) return BadRequest(result);
            return Ok(result);
        }

        /// <summary>
        /// Cập nhật mức độ yêu cầu của một kỹ năng đối với thiết kế nail.
        /// </summary>
        [HttpPut("{skillTypeId}")]
        [ProducesResponseType(typeof(ApiResult<NailRequiredSkillResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateRequiredSkillLevel(int variantId, Guid skillTypeId, [FromBody] UpdateRequiredSkillLevelRequest request)
        {
            var result = await _nailRequiredSkillService.UpdateRequiredSkillLevelAsync(variantId, skillTypeId, request);
            if (!result.IsSucceeded) return BadRequest(result);
            return Ok(result);
        }

        /// <summary>
        /// Xóa kỹ năng yêu cầu khỏi thiết kế nail.
        /// </summary>
        [HttpDelete("{skillTypeId}")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveRequiredSkill(int variantId, Guid skillTypeId)
        {
            var result = await _nailRequiredSkillService.DeleteRequiredSkillAsync(variantId, skillTypeId);
            if (!result.IsSucceeded) return BadRequest(result);
            return Ok(result);
        }
    }
}
