using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.CustomerNailRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Infrastructure.Service;

namespace Nailify.Capstone.Presentation.Controllers
{
    /// <summary>
    /// API quản lý mẫu nail khách hàng.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerNailsController : BaseApiController 
    {
        private readonly ICustomerNailService _customerNailService;
        private readonly CloudinaryService _cloudinaryService;
        private readonly IValidator<CustomerNailCreateRequest> _createValidator;
        private readonly IValidator<CustomerNailUpdateRequest> _updateValidator;

        public CustomerNailsController(
            ICustomerNailService customerNailService,
            CloudinaryService cloudinaryService,
            IValidator<CustomerNailCreateRequest> createValidator,
            IValidator<CustomerNailUpdateRequest> updateValidator)
        {
            _customerNailService = customerNailService;
            _cloudinaryService = cloudinaryService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>
        /// Lấy danh sách mẫu nail khách hàng phân trang, hỗ trợ lọc theo tên và danh mục.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<PagedList<CustomerNailDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? name = null,
            [FromQuery] bool? isPublic = null,
            [FromQuery] bool? isFavorite = null)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var result = await _customerNailService.GetPagedCustomerNailsAsync(
                    pageNumber, pageSize, currentUserId, name, isPublic, isFavorite);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return UnauthorizedResponse();
            }
        }

        /// <summary>
        /// Lấy chi tiết mẫu nail khách hàng theo ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResult<CustomerNailDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _customerNailService.GetCustomerNailByIdAsync(id);

            if (result.IsSucceeded && result.Data != null)
            {
                try
                {
                    var currentUserId = GetCurrentUserId();
                    if (!result.Data.IsPublic && result.Data.UserId != currentUserId)
                    {
                        return UnauthorizedResponse();
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    if (!result.Data.IsPublic)
                    {
                        return UnauthorizedResponse();
                    }
                }
            }

            return result.IsSucceeded ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Tạo mẫu nail khách hàng mới.
        /// </summary>
        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResult<CustomerNailDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm] CustomerNailCreateRequest request, IFormFile? image)
        {
            try
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
                    var result = await _customerNailService.CreateCustomerNailAsync(request, uploadedImageUrl, currentUserId);
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
                    return BadRequest(new ApiResult<object>(false, $"Tạo móng tùy chỉnh thất bại: {ex.Message}"));
                }
            }
            catch (UnauthorizedAccessException)
            {
                return UnauthorizedResponse();
            }
        }

        /// <summary>
        /// Cập nhật mẫu nail khách hàng.
        /// </summary>
        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResult<CustomerNailDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromForm] CustomerNailUpdateRequest request, IFormFile? image)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var existingResult = await _customerNailService.GetCustomerNailByIdAsync(id);
                if (!existingResult.IsSucceeded)
                {
                    return NotFound(existingResult);
                }

                if (existingResult.Data.UserId != currentUserId)
                {
                    return UnauthorizedResponse();
                }

                var validationResult = await _updateValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new ApiErrorResult<object>(validationResult.Errors.Select(error => error.ErrorMessage).ToList()));
                }

                var uploadedImageUrl = string.Empty;
                try
                {
                    uploadedImageUrl = await UploadImageAsync(image);

                    var result = await _customerNailService.UpdateCustomerNailAsync(id, request, uploadedImageUrl);
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
                    return BadRequest(new ApiResult<object>(false, $"Cập nhật thất bại: {ex.Message}"));
                }
            }
            catch (UnauthorizedAccessException)
            {
                return UnauthorizedResponse();
            }
        }

        /// <summary>
        /// Xóa mẫu nail khách hàng.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                var existingResult = await _customerNailService.GetCustomerNailByIdAsync(id);
                if (!existingResult.IsSucceeded)
                {
                    return NotFound(existingResult);
                }

                if (existingResult.Data.UserId != currentUserId)
                {
                    return UnauthorizedResponse();
                }

                var result = await _customerNailService.DeleteCustomerNailAsync(id);
                if (!result.IsSucceeded)
                {
                    return NotFound(result);
                }

                await DeleteImageAsync(existingResult.Data.ImageUrl);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return UnauthorizedResponse();
            }
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
