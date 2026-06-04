using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailComponentRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;

namespace Nailify.Capstone.Presentation.Controllers
{
    /// <summary>
    /// API quản lý thành phần trên móng.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class NailComponentsController : ControllerBase
    {
        private readonly INailComponentService _nailComponentService;
        private readonly IValidator<NailComponentCreateRequest> _createValidator;
        private readonly IValidator<NailComponentUpdateRequest> _updateValidator;

        public NailComponentsController(
            INailComponentService nailComponentService,
            IValidator<NailComponentCreateRequest> createValidator,
            IValidator<NailComponentUpdateRequest> updateValidator)
        {
            _nailComponentService = nailComponentService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>
        /// Lấy danh sách thành phần trên móng với phân trang. 
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<PagedList<NailComponentDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _nailComponentService.GetPagedNailComponentsAsync(pageNumber, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Lấy chi tiết thành phần trên móng theo ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResult<NailComponentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _nailComponentService.GetNailComponentByIdAsync(id);
            return result.IsSucceeded ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Tạo thành phần trên móng mới.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<NailComponentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] NailComponentCreateRequest request)
        {
            var validationResult = await _createValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ApiErrorResult<object>(validationResult.Errors.Select(error => error.ErrorMessage).ToList()));
            }

            var result = await _nailComponentService.CreateNailComponentAsync(request);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Cập nhật thành phần trên móng.
        /// </summary>
        [HttpPut]
        [ProducesResponseType(typeof(ApiResult<NailComponentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update([FromBody] NailComponentUpdateRequest request)
        {
            var validationResult = await _updateValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ApiErrorResult<object>(validationResult.Errors.Select(error => error.ErrorMessage).ToList()));
            }

            var result = await _nailComponentService.UpdateNailComponentAsync(request);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Xóa thành phần trên móng.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _nailComponentService.DeleteNailComponentAsync(id);
            return result.IsSucceeded ? Ok(result) : NotFound(result);
        }
    }
}
