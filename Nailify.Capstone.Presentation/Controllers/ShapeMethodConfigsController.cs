using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.ShapeMethodConfigRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Enums;

namespace Nailify.Capstone.Presentation.Controllers
{
    /// <summary>
    /// API quản lý cấu hình cách làm theo dạng móng.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ShapeMethodConfigsController : ControllerBase
    {
        private readonly IShapeMethodConfigService _shapeMethodConfigService;

        public ShapeMethodConfigsController(IShapeMethodConfigService shapeMethodConfigService)
        {
            _shapeMethodConfigService = shapeMethodConfigService;
        }

        /// <summary>
        /// Lấy danh sách cấu hình cách làm có phân trang, hỗ trợ lọc theo dạng móng và tên.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<PagedList<ShapeMethodConfigDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? nailShapeId = null,
            [FromQuery] string? name = null,
            [FromQuery] ActiveStatusFilter? status = null)
        {
            var statusStr = (status == null || status == ActiveStatusFilter.All) ? null : status.ToString();
            var result = await _shapeMethodConfigService.GetPagedShapeMethodConfigsAsync(pageNumber, pageSize, nailShapeId, name, statusStr);
            return Ok(result);
        }

        /// <summary>
        /// Lấy chi tiết cấu hình cách làm theo ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResult<ShapeMethodConfigDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _shapeMethodConfigService.GetShapeMethodConfigByIdAsync(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách cấu hình cách làm theo dạng móng.
        /// </summary>
        [HttpGet("nail-shape/{nailShapeId}")]
        [ProducesResponseType(typeof(ApiResult<List<ShapeMethodConfigDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByNailShapeId(int nailShapeId, [FromQuery] ActiveStatusFilter? status = null)
        {
            var statusStr = (status == null || status == ActiveStatusFilter.All) ? null : status.ToString();
            var result = await _shapeMethodConfigService.GetShapeMethodConfigsByNailShapeIdAsync(nailShapeId, statusStr);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Tạo mới cấu hình cách làm.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<ShapeMethodConfigDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] ShapeMethodConfigCreateRequest request)
        {
            var result = await _shapeMethodConfigService.CreateShapeMethodConfigAsync(request);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Cập nhật cấu hình cách làm.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResult<ShapeMethodConfigDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] ShapeMethodConfigUpdateRequest request)
        {
            var result = await _shapeMethodConfigService.UpdateShapeMethodConfigAsync(id, request);
            if (!result.IsSucceeded)
            {
                if (result.Message.Contains("không tìm thấy"))
                {
                    return NotFound(result);
                }

                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Xóa cấu hình cách làm.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _shapeMethodConfigService.DeleteShapeMethodConfigAsync(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}
