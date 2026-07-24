using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.PromotionRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Enums;
using Nailify.Capstone.Infrastructure.Service;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Nailify.Capstone.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionsController : ControllerBase
    {
        private readonly IPromotionService _promotionService;
        private readonly CloudinaryService _cloudinaryService;

        public PromotionsController(IPromotionService promotionService, CloudinaryService cloudinaryService)
        {
            _promotionService = promotionService;
            _cloudinaryService = cloudinaryService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<PagedList<PromotionDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] PromotionType? type = null,
            [FromQuery] PromotionScope? scope = null,
            [FromQuery] DiscountType? discountType = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var result = await _promotionService.GetPagedAsync(
                pageNumber,
                pageSize,
                type,
                scope,
                discountType,
                startDate,
                endDate);

            return Ok(result);
        }

        [HttpGet("today")]
        [ProducesResponseType(typeof(ApiResult<PagedList<PromotionDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTodayPaged(
           [FromQuery] int pageNumber = 1,
           [FromQuery] int pageSize = 10)
        {
            var result = await _promotionService.GetTodayPagedAsync(
                pageNumber,
                pageSize,
                PromotionType.Voucher,
                GetCurrentUserIdOrNull());

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResult<PromotionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<PromotionDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _promotionService.GetByIdAsync(id);
            return result.IsSucceeded ? Ok(result) : NotFound(result);
        }

        [HttpGet("by-category/{categoryId:int}")]
        [ProducesResponseType(typeof(ApiResult<List<PromotionDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByCategoryId(int categoryId)
        {
            var result = await _promotionService.GetByCategoryIdAsync(categoryId);
            return Ok(result);
        }

        [HttpGet("by-category-type/{categoryTypeId:int}")]
        [ProducesResponseType(typeof(ApiResult<List<PromotionDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByCategoryTypeId(int categoryTypeId)
        {
            var result = await _promotionService.GetByCategoryTypeIdAsync(categoryTypeId);
            return Ok(result);
        }

        [HttpGet("by-nail-design/{nailDesignId:int}")]
        [ProducesResponseType(typeof(ApiResult<List<PromotionDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByNailDesignId(int nailDesignId)
        {
            var result = await _promotionService.GetByNailDesignIdAsync(nailDesignId);
            return Ok(result);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResult<PromotionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<PromotionDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm] PromotionRequest request, IFormFile? image)
        {
            var uploadedImageUrl = string.Empty;
            try
            {
                uploadedImageUrl = await UploadImageAsync(image);

                var result = await _promotionService.CreateAsync(request, uploadedImageUrl);
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
                return BadRequest(new ApiResult<object>(false, $"Tao khuyen mai that bai khi tai anh: {ex.Message}"));
            }
        }

        [HttpPost("voucherForReschedule/{bookingId}")]
        [ProducesResponseType(typeof(ApiResult<PromotionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<PromotionDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddVoucherForReschedule(Guid bookingId)
        {
            var result = await _promotionService.AddVoucherForRescheduleAsync(bookingId);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id:int}")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResult<PromotionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<PromotionDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResult<PromotionDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromForm] PromotionRequest request, IFormFile? image)
        {
            var existingResult = await _promotionService.GetByIdAsync(id);
            if (!existingResult.IsSucceeded)
            {
                return NotFound(existingResult);
            }

            var uploadedImageUrl = string.Empty;
            try
            {
                uploadedImageUrl = await UploadImageAsync(image);

                var result = await _promotionService.UpdateAsync(id, request, uploadedImageUrl);
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
                return BadRequest(new ApiResult<object>(false, $"Cap nhat khuyen mai that bai khi tai anh: {ex.Message}"));
            }
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var existingResult = await _promotionService.GetByIdAsync(id);
            if (!existingResult.IsSucceeded)
            {
                return NotFound(existingResult);
            }

            var result = await _promotionService.DeleteAsync(id);
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

        private Guid? GetCurrentUserIdOrNull()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }
}
