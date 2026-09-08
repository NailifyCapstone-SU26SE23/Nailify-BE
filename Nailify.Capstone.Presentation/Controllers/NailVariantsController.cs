using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailVariantRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Infrastructure.Service;
using System.Security.Claims;

namespace Nailify.Capstone.Presentation.Controllers
{
    /// <summary>
    /// API quản lý biến thể móng.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class NailVariantsController : ControllerBase
    {
        private readonly INailVariantService _nailVariantService;
        private readonly CloudinaryService _cloudinaryService;
        private readonly IValidator<NailVariantCreateRequest> _createValidator;
        private readonly IValidator<NailVariantUpdateRequest> _updateValidator;

        public NailVariantsController(
            INailVariantService nailVariantService,
            CloudinaryService cloudinaryService,
            IValidator<NailVariantCreateRequest> createValidator,
            IValidator<NailVariantUpdateRequest> updateValidator)
        {
            _nailVariantService = nailVariantService;
            _cloudinaryService = cloudinaryService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>
        /// Lấy danh sách biến thể móng phân trang, hỗ trợ tìm theo tên và lọc theo màu nail.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<PagedList<NailVariantDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? nailDesignId = null,
            [FromQuery] string? name = null)
        {
            var result = await _nailVariantService.GetPagedNailVariantsAsync(pageNumber, pageSize, nailDesignId, name, GetCurrentUserIdOrNull());
            return Ok(result);
        }

        /// <summary>
        /// Lấy chi tiết biến thể móng theo ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResult<NailVariantDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _nailVariantService.GetNailVariantByIdAsync(id, GetCurrentUserIdOrNull());
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Lấy tổng quan đặt lịch, yêu thích và đánh giá của biến thể móng.
        /// </summary>
        [HttpGet("summary/{id}")]
        [ProducesResponseType(typeof(ApiResult<NailSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSummary(int id)
        {
            var result = await _nailVariantService.GetNailVariantSummaryAsync(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Tạo biến thể móng mới.
        /// </summary>
        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResult<NailVariantDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm] NailVariantCreateRequest request, IFormFile? image)
        {
            var validationResult = await _createValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ApiErrorResult<object>(validationResult.Errors.Select(error => error.ErrorMessage).ToList()));
            }

            var uploadedImageUrl = string.Empty;
            try
            {
                uploadedImageUrl = await UploadImageAsync(image);

                var result = await _nailVariantService.CreateNailVariantAsync(request, uploadedImageUrl);
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
                return BadRequest(new ApiResult<object>(false, $"Tao bien the mong that bai khi tai anh: {ex.Message}"));
            }
        }

        /// <summary>
        /// Cập nhật biến thể móng.
        /// </summary>
        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResult<NailVariantDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromForm] NailVariantUpdateRequest request, IFormFile? imageUrl)
        {
            var validationResult = await _updateValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ApiErrorResult<object>(validationResult.Errors.Select(error => error.ErrorMessage).ToList()));
            }

            var existingResult = await _nailVariantService.GetNailVariantByIdAsync(id);
            if (!existingResult.IsSucceeded)
            {
                return NotFound(existingResult);
            }

            var uploadedImageUrl = string.Empty;
            try
            {
                uploadedImageUrl = await UploadImageAsync(imageUrl);

                var result = await _nailVariantService.UpdateNailVariantAsync(id, request, uploadedImageUrl);
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
                return BadRequest(new ApiResult<object>(false, $"Cap nhat bien the mong that bai khi tai anh: {ex.Message}"));
            }
        }

        /// <summary>
        /// Xóa biến thể móng.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var existingResult = await _nailVariantService.GetNailVariantByIdAsync(id);
            if (!existingResult.IsSucceeded)
            {
                return NotFound(existingResult);
            }

            var result = await _nailVariantService.DeleteNailVariantAsync(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            await DeleteImageAsync(existingResult.Data.ImageUrl);
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách mẫu móng thợ có thể thực hiện.
        /// </summary>
        [HttpGet("capable-by-artist/{artistId}")]
        [ProducesResponseType(typeof(ApiResult<List<NailVariantDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCapableNailVariants(Guid artistId)
        {
            var result = await _nailVariantService.GetCapableNailVariantsAsync(artistId, GetCurrentUserIdOrNull());
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }
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
