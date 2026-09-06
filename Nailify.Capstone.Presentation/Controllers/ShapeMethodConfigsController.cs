using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.ShapeMethodConfigRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;

namespace Nailify.Capstone.Presentation.Controllers
{
    /// <summary>
    /// API quan ly cau hinh cach lam theo dang mong.
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
        /// Lay danh sach cau hinh cach lam phan trang, ho tro loc theo dang mong va ten.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<PagedList<ShapeMethodConfigDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? nailShapeId = null,
            [FromQuery] string? name = null)
        {
            var result = await _shapeMethodConfigService.GetPagedShapeMethodConfigsAsync(pageNumber, pageSize, nailShapeId, name);
            return Ok(result);
        }

        /// <summary>
        /// Lay chi tiet cau hinh cach lam theo ID.
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
        /// Lay danh sach cau hinh cach lam theo dang mong.
        /// </summary>
        [HttpGet("nail-shape/{nailShapeId}")]
        [ProducesResponseType(typeof(ApiResult<List<ShapeMethodConfigDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByNailShapeId(int nailShapeId)
        {
            var result = await _shapeMethodConfigService.GetShapeMethodConfigsByNailShapeIdAsync(nailShapeId);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Tao cau hinh cach lam moi.
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
        /// Cap nhat cau hinh cach lam.
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
                if (result.Message.Contains("Khong tim thay"))
                {
                    return NotFound(result);
                }

                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Xoa cau hinh cach lam.
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
