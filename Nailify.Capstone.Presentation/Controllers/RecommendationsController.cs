using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;

namespace Nailify.Capstone.Presentation.Controllers
{
    /// <summary>
    /// API quản lý các gợi ý thông minh dựa trên AI.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class RecommendationsController : BaseApiController
    {
        private readonly IRecommendationService _recommendationService;
        public RecommendationsController(IRecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
        }
        /// <summary>
        /// Lấy danh sách mẫu móng gợi ý cá nhân hóa cho khách hàng hiện tại.
        /// </summary>
        [HttpGet("for-me")]
        [ProducesResponseType(typeof(ApiResult<List<RecommendedNailVariantResponseDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetRecommendations([FromQuery] int limit = 10)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _recommendationService.GetRecommendationsAsync(userId, limit);
                if (!result.IsSucceeded)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return UnauthorizedResponse();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiErrorResult<object>($"Lỗi máy chủ: {ex.Message}"));
            }
        }
        /// <summary>
        /// Lấy danh sách nguồn cấp tin (Feed) gợi ý thông minh được phân trang.
        /// </summary>
        [HttpGet("feed")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(ApiResult<PagedList<RecommendedNailVariantResponseDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetRecommendationsFeed([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _recommendationService.GetRecommendationsFeedAsync(userId, pageNumber, pageSize);
                if (!result.IsSucceeded)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return UnauthorizedResponse();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiErrorResult<object>($"Lỗi máy chủ: {ex.Message}"));
            }
        }
    }
}
