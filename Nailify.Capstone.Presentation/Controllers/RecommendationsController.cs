using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;

namespace Nailify.Capstone.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecommendationsController : BaseApiController
    {
        private readonly INailRecommendationService _service;
        public RecommendationsController(INailRecommendationService service)
            => _service = service;
        /// <summary>
        /// [Admin] Train/Retrain ML model từ dữ liệu booking, favorite, rating hiện tại.
        /// </summary>
        [HttpPost("train")]
        [ProducesResponseType(typeof(ApiResult<ModelTrainingResultDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Train()
        {
            var result = await _service.TrainModelAsync();
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }
        /// <summary>
        /// [Customer] Lấy danh sách nail variant được gợi ý cá nhân hóa.
        /// Tự động fallback về popular nếu user mới (cold start).
        /// </summary>
        [HttpGet("nail-variants")]
        [ProducesResponseType(typeof(ApiResult<List<NailVariantRecommendationDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRecommendations([FromQuery] int topN = 10)
        {
            var result = await _service.GetRecommendationsAsync(GetCurrentUserId(), topN);
            return Ok(result);
        }
        /// <summary>
        /// [Public] Top nail variants phổ biến nhất (được đặt nhiều nhất).
        /// </summary>
        [HttpGet("popular")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResult<List<NailVariantRecommendationDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPopular([FromQuery] int topN = 10)
        {
            var result = await _service.GetPopularAsync(topN);
            return Ok(result);
        }
    }
}
