using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.QuizRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.QuizResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;

namespace Nailify.Capstone.Presentation.Controllers
{
    /// <summary>
    /// API quản lý Quiz trắc nghiệm phong cách và sở thích nail.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class QuizzesController : BaseApiController
    {
        private readonly IQuizService _quizService;
        private readonly IRecommendationService _recommendationService;
        public QuizzesController(IQuizService quizService, IRecommendationService recommendationService)
        {
            _quizService = quizService;
            _recommendationService = recommendationService;
        }
        /// <summary>
        /// Lấy danh sách câu hỏi Quiz đang hoạt động (Customer).
        /// </summary>
        [HttpGet("questions")]
        [ProducesResponseType(typeof(ApiResult<List<StyleQuizQuestionResponseDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetQuizQuestions()
        {
            var result = await _quizService.GetQuizQuestionsAsync();
            return Ok(result);
        }
        /// <summary>
        /// Gửi câu trả lời trắc nghiệm của khách hàng để lưu và tính toán gợi ý cá nhân hóa ngay lập tức.
        /// </summary>
        [HttpPost("submit")]
        [ProducesResponseType(typeof(ApiResult<List<RecommendedNailVariantResponseDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SubmitQuizAnswers([FromBody] SubmitQuizAnswersRequestDto request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _recommendationService.SubmitQuizAnswersAsync(userId, request);
                if (!result.IsSucceeded)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return UnauthorizedResponse();
            }
        }
        /// <summary>
        /// Tạo mới câu hỏi trắc nghiệm cùng các phương án trả lời
        /// </summary>
        [HttpPost("questions")]
        [ProducesResponseType(typeof(ApiResult<StyleQuizQuestionResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateQuestion([FromBody] CreateQuizQuestionRequestDTO request)
        {
            var result = await _quizService.CreateQuestionAsync(request);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        /// <summary>
        /// Cập nhật nội dung câu hỏi hoặc thay thế các phương án trả lời trắc nghiệm.
        /// </summary>
        [HttpPut("questions/{id}")]
        [ProducesResponseType(typeof(ApiResult<StyleQuizQuestionResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateQuestion(Guid id, [FromBody] UpdateQuizQuestionRequestDTO request)
        {
            var result = await _quizService.UpdateQuestionAsync(id, request);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        /// <summary>
        /// Xóa một câu hỏi trắc nghiệm (Admin).
        /// </summary>
        [HttpDelete("questions/{id}")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteQuestion(Guid id)
        {
            var result = await _quizService.DeleteQuestionAsync(id);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
