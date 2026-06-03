using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.CustomerNailComponentRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;

namespace Nailify.Capstone.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerNailComponentsController : ControllerBase
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

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResult<CustomerNailComponentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _customerNailComponentService.GetCustomerNailComponentByIdAsync(id);
            return result.IsSucceeded ? Ok(result) : NotFound(result);
        }

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

        [HttpPut]
        [ProducesResponseType(typeof(ApiResult<CustomerNailComponentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update([FromBody] CustomerNailComponentUpdateRequest request)
        {
            var validationResult = await _updateValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ApiErrorResult<object>(validationResult.Errors.Select(error => error.ErrorMessage).ToList()));
            }

            var result = await _customerNailComponentService.UpdateCustomerNailComponentAsync(request);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

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
