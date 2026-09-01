using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailDesignRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Infrastructure.Service;
using System.Security.Claims;


namespace Nailify.Capstone.Presentation.Controllers
{
    /// <summary>
    /// API quản lý mẫu nail.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class NailDesignsController : ControllerBase
    {
        private readonly INailDesignService _nailDesignService;
        private readonly CloudinaryService _cloudinaryService;
        private readonly IValidator<NailDesignCreateRequest> _designCreateValidator;
        private readonly IValidator<NailDesignUpdateRequest> _designUpdateValidator;

        public NailDesignsController(
            INailDesignService nailDesignService,
            CloudinaryService cloudinaryService,
            IValidator<NailDesignCreateRequest> designCreateValidator,
            IValidator<NailDesignUpdateRequest> designUpdateValidator)
        {
            _nailDesignService = nailDesignService;
            _cloudinaryService = cloudinaryService;
            _designCreateValidator = designCreateValidator;
            _designUpdateValidator = designUpdateValidator;
        }

        /// <summary>
        /// Lấy danh sách mẫu nail phân trang, hỗ trợ lọc theo tên và danh mục.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<PagedList<NailDesignDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? name = null,
            [FromQuery] List<int>? categoryIds = null)
        {
            var result = await _nailDesignService.GetPagedNailDesignsAsync(pageNumber, pageSize, name, categoryIds, GetCurrentUserIdOrNull());
            return Ok(result);
        }

        /// <summary>
        /// Lấy chi tiết mẫu nail theo ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResult<NailDesignDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _nailDesignService.GetNailDesignByIdAsync(id, GetCurrentUserIdOrNull());
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Tạo mẫu nail mới.
        /// </summary>
        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResult<NailDesignDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm] NailDesignCreateRequest request, IFormFile? image)
        {
            var validationResult = await _designCreateValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ApiErrorResult<object>(validationResult.Errors.Select(error => error.ErrorMessage).ToList()));
            }

            var uploadedImageUrl = string.Empty;

            try
            {
                uploadedImageUrl = await UploadImageAsync(image);
                var result = await _nailDesignService.CreateNailDesignAsync(request, uploadedImageUrl);
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
                return BadRequest(new ApiResult<object>(false, $"Tạo mẫu nail thất bại khi tải ảnh: {ex.Message}"));
            }
        }

        /// <summary>
        /// Cập nhật mẫu nail.
        /// </summary>
        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResult<NailDesignDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromForm] NailDesignUpdateRequest request, IFormFile? image)
        {
            var validationResult = await _designUpdateValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ApiErrorResult<object>(validationResult.Errors.Select(error => error.ErrorMessage).ToList()));
            }

            var existingResult = await _nailDesignService.GetNailDesignByIdAsync(id);
            if (!existingResult.IsSucceeded)
            {
                return NotFound(existingResult);
            }

            var uploadedImageUrl = string.Empty;

            try
            {
                uploadedImageUrl = await UploadImageAsync(image);
                var result = await _nailDesignService.UpdateNailDesignAsync(id, request, uploadedImageUrl);
                if (!result.IsSucceeded)
                {
                    await DeleteImageAsync(uploadedImageUrl);
                    return BadRequest(result);
                }

                var oldImageUrl = existingResult.Data.ImageUrl;
                var currentImageUrl = result.Data.ImageUrl;
                if (!string.IsNullOrWhiteSpace(oldImageUrl)
                    && !string.Equals(oldImageUrl, currentImageUrl, StringComparison.Ordinal))
                {
                    await DeleteImageAsync(oldImageUrl);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                await DeleteImageAsync(uploadedImageUrl);
                return BadRequest(new ApiResult<object>(false, $"Cập nhật mẫu nail thất bại khi tải ảnh: {ex.Message}"));
            }
        }

        /// <summary>
        /// Xóa mẫu nail.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var existingResult = await _nailDesignService.GetNailDesignByIdAsync(id);
            if (!existingResult.IsSucceeded)
            {
                return NotFound(existingResult);
            }

            var result = await _nailDesignService.DeleteNailDesignAsync(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            await DeleteImageAsync(existingResult.Data.ImageUrl);
            return Ok(result);
        }

        /// <summary>
        /// Lấy mẫu nail theo danh mục.
        /// </summary>
        [HttpGet("by-category/{categoryId}")]
        [ProducesResponseType(typeof(ApiResult<List<NailDesignDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            var result = await _nailDesignService.GetNailDesignsByCategoryAsync(categoryId, GetCurrentUserIdOrNull());
            return Ok(result);
        }

        private Guid? GetCurrentUserIdOrNull()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
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
