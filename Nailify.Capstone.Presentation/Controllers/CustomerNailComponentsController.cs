using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.CustomerNailComponentRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;

namespace Nailify.Capstone.Presentation.Controllers
{
    /// <summary>
    /// API quản lý mẫu thành phần nail khách hàng.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerNailComponentsController : BaseApiController
    {
        private readonly ICustomerNailComponentService _customerNailComponentService;
        private readonly IValidator<CustomerNailComponentCreateRequest> _createValidator;
        private readonly IValidator<CustomerNailComponentUpdateRequest> _updateValidator;

        public CustomerNailComponentsController(
            ICustomerNailComponentService customerNailComponentService,
            IValidator<CustomerNailComponentCreateRequest> createValidator,
            IValidator<CustomerNailComponentUpdateRequest> updateValidator)
        {
            _customerNailComponentService = customerNailComponentService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>
        /// Lấy danh sách thành phần nail khách hàng phân trang, hỗ trợ lọc theo tên và danh mục.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<PagedList<CustomerNailComponentDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? customerNailId = null)
        {
            var result = await _customerNailComponentService.GetPagedCustomerNailComponentsAsync(pageNumber, pageSize, customerNailId);
            return Ok(result);
        }

        /// <summary>
        /// Lấy chi tiết thành phần nail khách hàng.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResult<CustomerNailComponentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _customerNailComponentService.GetCustomerNailComponentByIdAsync(id);
            return result.IsSucceeded ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Tạo thành phần nail khách hàng.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<CustomerNailComponentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CustomerNailComponentCreateRequest request)
        {
            var validationResult = await _createValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ApiErrorResult<object>(validationResult.Errors.Select(error => error.ErrorMessage).ToList()));
            }

            var result = await _customerNailComponentService.CreateCustomerNailComponentAsync(request);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Cập nhật thành phần nail khách hàng.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResult<CustomerNailComponentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] CustomerNailComponentUpdateRequest request)
        {
            var validationResult = await _updateValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ApiErrorResult<object>(validationResult.Errors.Select(error => error.ErrorMessage).ToList()));
            }

            var result = await _customerNailComponentService.UpdateCustomerNailComponentAsync(id, request);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Xóa thành phần nail khách hàng.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _customerNailComponentService.DeleteCustomerNailComponentAsync(id);
            return result.IsSucceeded ? Ok(result) : NotFound(result);
        }
    }
}
