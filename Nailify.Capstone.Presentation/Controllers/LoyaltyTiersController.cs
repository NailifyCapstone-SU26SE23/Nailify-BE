using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.DTOs.RequestDTOs.LoyaltyTierRequestDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Enums;
using Nailify.Capstone.Infrastructure.Service;

namespace Nailify.Capstone.Presentation.Controllers
{
    [Route("api/[controller]")]
    public class LoyaltyTiersController : BaseApiController
    {
        private readonly ILoyaltyTierService _service;
        private readonly CloudinaryService _cloudinaryService;

        public LoyaltyTiersController(
            ILoyaltyTierService service,
            CloudinaryService cloudinaryService)
        {
            _service = service;
            _cloudinaryService = cloudinaryService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [AllowAnonymous]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result.IsSucceeded ? Ok(result) : NotFound(result);
        }

        [Authorize]
        [HttpGet("/api/Loyalty/me")]
        public async Task<IActionResult> GetMyLoyalty()
        {
            var result = await _service.GetMyLoyaltyAsync(GetCurrentUserId());
            return result.IsSucceeded ? Ok(result) : NotFound(result);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create(
            [FromForm] LoyaltyTierRequest request,
            IFormFile? image)
        {
            string? uploadedImageUrl = null;

            try
            {
                if (image is { Length: > 0 })
                {
                    uploadedImageUrl = await _cloudinaryService.UploadImageAsync(image);
                }

                var result = await _service.CreateAsync(request, uploadedImageUrl);
                if (!result.IsSucceeded && uploadedImageUrl != null)
                {
                    await _cloudinaryService.DeleteImageAsync(uploadedImageUrl);
                }

                return result.IsSucceeded ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                if (uploadedImageUrl != null)
                {
                    await _cloudinaryService.DeleteImageAsync(uploadedImageUrl);
                }

                return BadRequest(new { isSucceeded = false, message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(
            int id,
            [FromForm] LoyaltyTierRequest request,
            IFormFile? image)
        {
            var existingResult = await _service.GetByIdAsync(id);
            if (!existingResult.IsSucceeded)
            {
                return NotFound(existingResult);
            }

            var oldImageUrl = existingResult.Data.ImageUrl;
            string? uploadedImageUrl = null;

            try
            {
                if (image is { Length: > 0 })
                {
                    uploadedImageUrl = await _cloudinaryService.UploadImageAsync(image);
                }

                var result = await _service.UpdateAsync(id, request, uploadedImageUrl);
                if (!result.IsSucceeded)
                {
                    if (uploadedImageUrl != null)
                    {
                        await _cloudinaryService.DeleteImageAsync(uploadedImageUrl);
                    }

                    return BadRequest(result);
                }

                if (uploadedImageUrl != null &&
                    !string.IsNullOrWhiteSpace(oldImageUrl) &&
                    !string.Equals(oldImageUrl, uploadedImageUrl, StringComparison.Ordinal))
                {
                    await _cloudinaryService.DeleteImageAsync(oldImageUrl);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                if (uploadedImageUrl != null)
                {
                    await _cloudinaryService.DeleteImageAsync(uploadedImageUrl);
                }

                return BadRequest(new { isSucceeded = false, message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return result.IsSucceeded ? Ok(result) : NotFound(result);
        }
    }
}
