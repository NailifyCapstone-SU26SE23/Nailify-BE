using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRatingRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingRatingResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Infrastructure.Service;

namespace Nailify.Capstone.Presentation.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class BookingRatingsController : BaseApiController
    {
        private readonly IBookingRatingService _bookingRatingService;
        private readonly CloudinaryService _cloudinaryService;

        public BookingRatingsController(IBookingRatingService bookingRatingService, CloudinaryService cloudinaryService)
        {
            _bookingRatingService = bookingRatingService;
            _cloudinaryService = cloudinaryService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<PagedList<BookingRatingResponseDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] BookingRatingRequestParameters parameters)
            => Ok(await _bookingRatingService.GetAllAsync(parameters));

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResult<BookingRatingResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _bookingRatingService.GetByIdAsync(id);
            return result.IsSucceeded ? Ok(result) : NotFound(result);
        }

        [HttpGet("by-booking/{bookingId:guid}")]
        [ProducesResponseType(typeof(ApiResult<BookingRatingResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByBookingId(Guid bookingId)
        {
            var result = await _bookingRatingService.GetByBookingIdAsync(bookingId);
            return result.IsSucceeded ? Ok(result) : NotFound(result);
        }

        [HttpGet("by-salon/{salonId:guid}")]
        [ProducesResponseType(typeof(ApiResult<PagedList<BookingRatingResponseDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBySalonId(Guid salonId, [FromQuery] BookingRatingRequestParameters parameters)
            => Ok(await _bookingRatingService.GetBySalonIdAsync(salonId, parameters));

        [HttpGet("by-nail-artist/{nailArtistId:guid}")]
        [ProducesResponseType(typeof(ApiResult<PagedList<BookingRatingResponseDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByNailArtistId(Guid nailArtistId, [FromQuery] BookingRatingRequestParameters parameters)
            => Ok(await _bookingRatingService.GetByNailArtistIdAsync(nailArtistId, parameters));

        [HttpGet("me")]
        [ProducesResponseType(typeof(ApiResult<PagedList<BookingRatingResponseDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByCustomerId([FromQuery] BookingRatingRequestParameters parameters)
        {
            var customerId = GetCurrentUserId();
            return Ok(await _bookingRatingService.GetByCustomerIdAsync(customerId, parameters));
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResult<BookingRatingResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm] BookingRatingCreateRequest request, IFormFile? image)
        {
            string? imageUrl = null;
            try
            {
                imageUrl = await UploadImageAsync(image);
                var result = await _bookingRatingService.CreateAsync(GetCurrentUserId(), request, imageUrl);
                if (result.IsSucceeded) return Ok(result);

                await DeleteImageAsync(imageUrl);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                await DeleteImageAsync(imageUrl);
                return BadRequest(new ApiErrorResult<object>($"Tao danh gia that bai khi tai anh: {ex.Message}"));
            }
        }

        [HttpPut("{id:guid}")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResult<BookingRatingResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(Guid id, [FromForm] BookingRatingUpdateRequest request, IFormFile? image)
        {
            string? imageUrl = null;
            try
            {
                imageUrl = await UploadImageAsync(image);
                var result = await _bookingRatingService.UpdateAsync(GetCurrentUserId(), id, request, imageUrl);
                if (result.IsSucceeded)
                {
                    return Ok(result);
                }

                await DeleteImageAsync(imageUrl);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                await DeleteImageAsync(imageUrl);
                return BadRequest(new ApiErrorResult<object>($"Cap nhat danh gia that bai khi tai anh: {ex.Message}"));
            }
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existing = await _bookingRatingService.GetByIdAsync(id);
            if (!existing.IsSucceeded) return NotFound(existing);

            var result = await _bookingRatingService.DeleteAsync(GetCurrentUserId(), id);
            if (!result.IsSucceeded) return BadRequest(result);

            await DeleteImageAsync(existing.Data.ImageUrl);
            return Ok(result);
        }

        private async Task<string?> UploadImageAsync(IFormFile? image)
        {
            return image is { Length: > 0 }
                ? await _cloudinaryService.UploadImageAsync(image)
                : null;
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
