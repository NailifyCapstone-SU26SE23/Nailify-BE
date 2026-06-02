using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailShapeRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Infrastructure.Service;

namespace Nailify.Capstone.Presentation.Controllers
{
    /// <summary>
    /// API quản lý dáng móng
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class NailShapesController : ControllerBase
    {
        private readonly INailShapeService _nailShapeService;
        private readonly CloudinaryService _cloudinaryService;
        private readonly IValidator<NailShapeCreateRequest> _createValidator;
        private readonly IValidator<NailShapeUpdateRequest> _updateValidator;

        public NailShapesController(
            INailShapeService nailShapeService,
            CloudinaryService cloudinaryService,
            IValidator<NailShapeCreateRequest> createValidator,
            IValidator<NailShapeUpdateRequest> updateValidator)
        {
            _nailShapeService = nailShapeService;
            _cloudinaryService = cloudinaryService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>
        /// Lấy danh sách dáng móng phân trang, hỗ trợ tìm theo tên.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<PagedList<NailShapeDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? name = null)
        {
            var result = await _nailShapeService.GetPagedNailShapesAsync(pageNumber, pageSize, name);
            return Ok(result);
        }

        /// <summary>
        /// Lấy chi tiết dáng móng theo ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResult<NailShapeDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _nailShapeService.GetNailShapeByIdAsync(id);
            return result.IsSucceeded ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Tạo dáng móng mới.
        /// </summary>
        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResult<NailShapeDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm] NailShapeCreateRequest request, IFormFile? image)
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

                var result = await _nailShapeService.CreateNailShapeAsync(request, uploadedImageUrl);
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
                return BadRequest(new ApiResult<object>(false, $"Tao dang mong that bai khi tai anh: {ex.Message}"));
            }
        }

        /// <summary>
        /// Cập nhật dáng móng.
        /// </summary>
        [HttpPut]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResult<NailShapeDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update([FromForm] NailShapeUpdateRequest request, IFormFile? image)
        {
            var validationResult = await _updateValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ApiErrorResult<object>(validationResult.Errors.Select(error => error.ErrorMessage).ToList()));
            }

            var existingResult = await _nailShapeService.GetNailShapeByIdAsync(request.NailShapeId);
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

                var result = await _nailShapeService.UpdateNailShapeAsync(request);
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
                return BadRequest(new ApiResult<object>(false, $"Cap nhat dang mong that bai khi tai anh: {ex.Message}"));
            }
        }

        /// <summary>
        /// Xóa dáng móng.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var existingResult = await _nailShapeService.GetNailShapeByIdAsync(id);
            if (!existingResult.IsSucceeded)
            {
                return NotFound(existingResult);
            }

            var result = await _nailShapeService.DeleteNailShapeAsync(id);
            if (result.IsSucceeded)
            {
                await DeleteImageAsync(existingResult.Data.ImageUrl);
            }

            return result.IsSucceeded ? Ok(result) : NotFound(result);
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
