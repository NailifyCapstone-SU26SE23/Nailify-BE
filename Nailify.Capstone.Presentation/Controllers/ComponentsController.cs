using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.ComponentRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Infrastructure.Service;

namespace Nailify.Capstone.Presentation.Controllers
{
    /// <summary>
    /// API quản lý thành phần trang trí móng.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ComponentsController : ControllerBase
    {
        private readonly IComponentService _componentService;
        private readonly CloudinaryService _cloudinaryService;
        private readonly IValidator<ComponentCreateRequest> _createValidator;
        private readonly IValidator<ComponentUpdateRequest> _updateValidator;

        public ComponentsController(
            IComponentService componentService,
            CloudinaryService cloudinaryService,
            IValidator<ComponentCreateRequest> createValidator,
            IValidator<ComponentUpdateRequest> updateValidator)
        {
            _componentService = componentService;
            _cloudinaryService = cloudinaryService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>
        /// Lấy danh sách thành phần phân trang, hỗ trợ tìm theo tên và loại thành phần.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<PagedList<ComponentDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? name = null,
            [FromQuery] ComponentType? componentType = null)
        {
            var result = await _componentService.GetPagedComponentsAsync(pageNumber, pageSize, name, componentType);
            return Ok(result);
        }

        /// <summary>
        /// Lấy chi tiết thành phần theo ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResult<ComponentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _componentService.GetComponentByIdAsync(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Tạo thành phần mới.
        /// </summary>
        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResult<ComponentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm] ComponentCreateRequest request, IFormFile? image)
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

                var result = await _componentService.CreateComponentAsync(request, uploadedImageUrl);
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
                return BadRequest(new ApiResult<object>(false, $"Tao thanh phan that bai khi tai anh: {ex.Message}"));
            }
        }

        /// <summary>
        /// Cập nhật thành phần.
        /// </summary>
        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResult<ComponentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromForm] ComponentUpdateRequest request, IFormFile? image)
        {
            var validationResult = await _updateValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ApiErrorResult<object>(validationResult.Errors.Select(error => error.ErrorMessage).ToList()));
            }

            var existingResult = await _componentService.GetComponentByIdAsync(id);
            if (!existingResult.IsSucceeded)
            {
                return NotFound(existingResult);
            }

            var uploadedImageUrl = string.Empty;
            try
            {
                uploadedImageUrl = await UploadImageAsync(image);

                var result = await _componentService.UpdateComponentAsync(id, request, uploadedImageUrl);
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
                return BadRequest(new ApiResult<object>(false, $"Cap nhat thanh phan that bai khi tai anh: {ex.Message}"));
            }
        }

        /// <summary>
        /// Xóa thành phần.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var existingResult = await _componentService.GetComponentByIdAsync(id);
            if (!existingResult.IsSucceeded)
            {
                return NotFound(existingResult);
            }

            var result = await _componentService.DeleteComponentAsync(id);
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
