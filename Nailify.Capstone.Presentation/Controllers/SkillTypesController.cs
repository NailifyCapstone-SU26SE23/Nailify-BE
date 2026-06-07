using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.SkillTypeRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.SkillTypeResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;

namespace Nailify.Capstone.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkillTypesController : ControllerBase
    {
       private readonly ISkillTypeService _skillTypeService;
        public SkillTypesController(ISkillTypeService skillTypeService)
        {
            _skillTypeService = skillTypeService;
        }
        /// <summary>
        /// Lấy danh sách loại kỹ năng (phân trang).
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<PagedList<SkillTypeResponseDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? name = null)
        {
            var response = await _skillTypeService.GetPagedSkillTypesAsync(pageNumber, pageSize, name);
            return Ok(response);
        }
        /// <summary>
        /// Lấy chi tiết thông tin một loại kỹ năng theo ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResult<SkillTypeResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _skillTypeService.GetSkillTypeByIdAsync(id);
            if (!response.IsSucceeded) return NotFound(response);
            return Ok(response);
        }
        /// <summary>
        /// Tạo mới một loại kỹ năng.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<SkillTypeResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] SkillTypeCreateRequest request)
        {
            var response = await _skillTypeService.CreateSkillTypeAsync(request);
            if (!response.IsSucceeded) return BadRequest(response);
            return Ok(response);
        }
        /// <summary>
        /// Cập nhật thông tin loại kỹ năng.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResult<SkillTypeResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] SkillTypeUpdateRequest request)
        {
            var response = await _skillTypeService.UpdateSkillTypeAsync(id, request);
            if (!response.IsSucceeded) return BadRequest(response);
            return Ok(response);
        }
        /// <summary>
        /// Xóa (vô hiệu hóa) một loại kỹ năng.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _skillTypeService.DeleteSkillTYpeAsync(id);
            if (!result.IsSucceeded) return NotFound(result);
            return Ok(result);
        }
    }
}
