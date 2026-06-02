using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailDesignRequestDTOs;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailVariantRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Infrastructure.Service;
using System;
using System.Collections.Generic;
using System.Linq;

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
        private readonly INailVariantService _nailVariantService;
        private readonly CloudinaryService _cloudinaryService;
        private readonly IValidator<NailVariantCreateRequest> _variantCreateValidator;
        private readonly IValidator<NailVariantUpdateRequest> _variantUpdateValidator;
        private readonly IValidator<NailDesignCreateRequest> _designCreateValidator;
        private readonly IValidator<NailDesignUpdateRequest> _designUpdateValidator;

        public NailDesignsController(
            INailDesignService nailDesignService,
            INailVariantService nailVariantService,
            CloudinaryService cloudinaryService,
            IValidator<NailVariantCreateRequest> variantCreateValidator,
            IValidator<NailVariantUpdateRequest> variantUpdateValidator,
            IValidator<NailDesignCreateRequest> designCreateValidator,
            IValidator<NailDesignUpdateRequest> designUpdateValidator)
        {
            _nailDesignService = nailDesignService;
            _nailVariantService = nailVariantService;
            _cloudinaryService = cloudinaryService;
            _variantCreateValidator = variantCreateValidator;
            _variantUpdateValidator = variantUpdateValidator;
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
            var result = await _nailDesignService.GetPagedNailDesignsAsync(pageNumber, pageSize, name, categoryIds);
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
            var result = await _nailDesignService.GetNailDesignByIdAsync(id);
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
        public async Task<IActionResult> Create([FromForm] NailDesignCreateRequest request, List<IFormFile>? images)
        {
            var validationResult = await _designCreateValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ApiErrorResult<object>(validationResult.Errors.Select(error => error.ErrorMessage).ToList()));
            }

            var uploadedImageUrls = new List<string>();

            try
            {
                uploadedImageUrls = await UploadImagesAsync(images);
                var result = await _nailDesignService.CreateNailDesignAsync(request, uploadedImageUrls);
                if (!result.IsSucceeded)
                {
                    await DeleteImagesAsync(uploadedImageUrls);
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                await DeleteImagesAsync(uploadedImageUrls);
                return BadRequest(new ApiResult<object>(false, $"Tạo mẫu nail thất bại khi tải ảnh: {ex.Message}"));
            }
        }

        /// <summary>
        /// Cập nhật mẫu nail.
        /// </summary>
        [HttpPut]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResult<NailDesignDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromForm] NailDesignUpdateRequest request, List<IFormFile>? images)
        {
            var validationResult = await _designUpdateValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ApiErrorResult<object>(validationResult.Errors.Select(error => error.ErrorMessage).ToList()));
            }

            var existingResult = await _nailDesignService.GetNailDesignByIdAsync(request.NailDesignId);
            if (!existingResult.IsSucceeded)
            {
                return NotFound(existingResult);
            }

            var uploadedImageUrls = new List<string>();

            try
            {
                uploadedImageUrls = await UploadImagesAsync(images);
                var result = await _nailDesignService.UpdateNailDesignAsync(request, uploadedImageUrls);
                if (!result.IsSucceeded)
                {
                    await DeleteImagesAsync(uploadedImageUrls);
                    return BadRequest(result);
                }

                var keptImageUrls = request.ExistingImageUrls.Distinct().ToList();
                var removedImageUrls = existingResult.Data.ImageUrls
                    .Except(keptImageUrls)
                    .ToList();
                await DeleteImagesAsync(removedImageUrls);

                return Ok(result);
            }
            catch (Exception ex)
            {
                await DeleteImagesAsync(uploadedImageUrls);
                return BadRequest(new ApiResult<object>(false, $"Cập nhật mẫu nail thất bại khi tải ảnh: {ex.Message}"));
            }
        }

        /// <summary>
        /// Xóa mẫu nail bằng cách chuyển trạng thái sang InActive.
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

            await DeleteImagesAsync(existingResult.Data.ImageUrls);
            return Ok(result);
        }

        /// <summary>
        /// Lấy mẫu nail theo danh mục.
        /// </summary>
        [HttpGet("by-category/{categoryId}")]
        [ProducesResponseType(typeof(ApiResult<List<NailDesignDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            var result = await _nailDesignService.GetNailDesignsByCategoryAsync(categoryId);
            return Ok(result);
        }

        private async Task<List<string>> UploadImagesAsync(List<IFormFile>? images)
        {
            var validImages = images?
                .Where(image => image != null && image.Length > 0)
                .ToList() ?? new List<IFormFile>();

            if (!validImages.Any())
            {
                return new List<string>();
            }

            return await _cloudinaryService.UploadMultipleImagesAsync(validImages);
        }

        private async Task DeleteImagesAsync(IEnumerable<string>? imageUrls)
        {
            var urls = imageUrls?
                .Where(imageUrl => !string.IsNullOrWhiteSpace(imageUrl))
                .Distinct()
                .ToList() ?? new List<string>();

            if (urls.Any())
            {
                await _cloudinaryService.DeleteMultipleImagesAsync(urls);
            }
        }
    }
}
