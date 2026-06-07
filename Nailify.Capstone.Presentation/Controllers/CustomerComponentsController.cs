using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.CustomerComponentRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Infrastructure.Service;

namespace Nailify.Capstone.Presentation.Controllers
{
    /// <summary>
    /// API quản lý thành phần móng của khách hàng.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerComponentsController : BaseApiController
    {
        private readonly ICustomerComponentService _customerComponentService;
        private readonly CloudinaryService _cloudinaryService;
        private readonly IValidator<CustomerComponentCreateRequest> _createValidator;
        private readonly IValidator<CustomerComponentUpdateRequest> _updateValidator;

        public CustomerComponentsController(
            ICustomerComponentService customerComponentService,
            CloudinaryService cloudinaryService,
            IValidator<CustomerComponentCreateRequest> createValidator,
            IValidator<CustomerComponentUpdateRequest> updateValidator)
        {
            _customerComponentService = customerComponentService;
            _cloudinaryService = cloudinaryService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>
        /// Lấy danh sách thành phần móng khách hàng với phân trang. 
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<PagedList<CustomerComponentDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? name = null,
            [FromQuery] ComponentType? componentType = null)
        {
            var currentUserId = GetCurrentUserId();
            var result = await _customerComponentService.GetPagedCustomerComponentsAsync(pageNumber, pageSize, currentUserId, name, componentType);
            return Ok(result);
        }

        /// <summary>
        /// Lấy chi tiết thành phần móng khách hàng. 
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResult<CustomerComponentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _customerComponentService.GetCustomerComponentByIdAsync(id);
            return result.IsSucceeded ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Tạo thành phần trên móng khách hàng. 
        /// </summary>
        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResult<CustomerComponentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm] CustomerComponentCreateRequest request, IFormFile? image)
        {
            var currentUserId = GetCurrentUserId();

            var validationResult = await _createValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ApiErrorResult<object>(validationResult.Errors.Select(error => error.ErrorMessage).ToList()));
            }

            var uploadedImageUrl = string.Empty;
            try
            {
                uploadedImageUrl = await UploadImageAsync(image);
                var result = await _customerComponentService.CreateCustomerComponentAsync(request, uploadedImageUrl, currentUserId);
                if (!result.IsSucceeded)
                {
                    await DeleteImageAsync(uploadedImageUrl);
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                await DeleteImageAsync(uploadedImageUrl);
                return BadRequest(new ApiResult<object>(false, $"Tạo thành phần tùy chỉnh thất bại khi tải ảnh: {ex.Message}"));
            }
        }

        /// <summary>
        /// Cập nhật thành phần trên móng khách hàng. 
        /// </summary>
        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResult<CustomerComponentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromForm] CustomerComponentUpdateRequest request, IFormFile? image)
        {
            var validationResult = await _updateValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ApiErrorResult<object>(validationResult.Errors.Select(error => error.ErrorMessage).ToList()));
            }

            var existingResult = await _customerComponentService.GetCustomerComponentByIdAsync(id);
            if (!existingResult.IsSucceeded)
            {
                return NotFound(existingResult);
            }

            var uploadedImageUrl = string.Empty;
            try
            {
                uploadedImageUrl = await UploadImageAsync(image);
                request.ImageUrl = string.IsNullOrWhiteSpace(uploadedImageUrl)
                    ? existingResult.Data.ImageUrl
                    : uploadedImageUrl;

                var result = await _customerComponentService.UpdateCustomerComponentAsync(id, request);
                if (!result.IsSucceeded)
                {
                    await DeleteImageAsync(uploadedImageUrl);
                    return BadRequest(result);
                }

                if (!string.IsNullOrWhiteSpace(uploadedImageUrl))
                {
                    await DeleteImageAsync(existingResult.Data.ImageUrl);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                await DeleteImageAsync(uploadedImageUrl);
                return BadRequest(new ApiResult<object>(false, $"Cập nhật thành phần tùy chỉnh thất bại khi tải ảnh: {ex.Message}"));
            }
        }

        /// <summary>
        /// Xóa thành phần trên móng khách hàng. 
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var existingResult = await _customerComponentService.GetCustomerComponentByIdAsync(id);
            if (!existingResult.IsSucceeded)
            {
                return NotFound(existingResult);
            }

            var result = await _customerComponentService.DeleteCustomerComponentAsync(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            await DeleteImageAsync(existingResult.Data.ImageUrl);
            return Ok(result);
        }

        private async Task<string> UploadImageAsync(IFormFile? image)
        {
            if (image == null || image.Length == 0)
            {
                return string.Empty;
            }

            return await _cloudinaryService.UploadImageAsync(image);
        }

        private async Task DeleteImageAsync(string? imageUrl)
        {
            if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                await _cloudinaryService.DeleteImageAsync(imageUrl);
            }
        }
    }
}
