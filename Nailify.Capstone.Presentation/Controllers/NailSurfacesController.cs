using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailSurfaceRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;

namespace Nailify.Capstone.Presentation.Controllers
{
    /// <summary>
    /// API quản lý bề mặt móng
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class NailSurfacesController : ControllerBase
    {
        private readonly INailSurfaceService _nailSurfaceService;
        private readonly IValidator<NailSurfaceCreateRequest> _createValidator;
        private readonly IValidator<NailSurfaceUpdateRequest> _updateValidator;

        public NailSurfacesController(
            INailSurfaceService nailSurfaceService,
            IValidator<NailSurfaceCreateRequest> createValidator,
            IValidator<NailSurfaceUpdateRequest> updateValidator)
        {
            _nailSurfaceService = nailSurfaceService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>
        /// Lấy danh sách bề mặt móng phân trang, hỗ trợ tìm theo tên.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<PagedList<NailSurfaceDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? name = null)
        {
            var result = await _nailSurfaceService.GetPagedNailSurfacesAsync(pageNumber, pageSize, name);
            return Ok(result);
        }

        /// <summary>
        /// Lấy chi tiết bề mặt móng theo ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResult<NailSurfaceDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _nailSurfaceService.GetNailSurfaceByIdAsync(id);
            return result.IsSucceeded ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Tạo bề mặt móng mới.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<NailSurfaceDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] NailSurfaceCreateRequest request)
        {
            var validationResult = await _createValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ApiErrorResult<object>(validationResult.Errors.Select(error => error.ErrorMessage).ToList()));
            }

            var result = await _nailSurfaceService.CreateNailSurfaceAsync(request);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Cập nhật bề mặt móng.
        /// </summary>
        [HttpPut]
        [ProducesResponseType(typeof(ApiResult<NailSurfaceDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update([FromBody] NailSurfaceUpdateRequest request)
        {
            var validationResult = await _updateValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ApiErrorResult<object>(validationResult.Errors.Select(error => error.ErrorMessage).ToList()));
            }

            var result = await _nailSurfaceService.UpdateNailSurfaceAsync(request);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Xóa bề mặt móng.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _nailSurfaceService.DeleteNailSurfaceAsync(id);
            return result.IsSucceeded ? Ok(result) : NotFound(result);
        }
    }
}
