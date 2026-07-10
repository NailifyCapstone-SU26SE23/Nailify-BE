using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.QuizRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.QuizResponseDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface IQuizService
    {
        Task<ApiResult<List<StyleQuizQuestionResponseDTO>>> GetQuizQuestionsAsync();
        Task<ApiResult<StyleQuizQuestionResponseDTO>> CreateQuestionAsync(CreateQuizQuestionRequestDTO request);
        Task<ApiResult<StyleQuizQuestionResponseDTO>> UpdateQuestionAsync(Guid questionId, UpdateQuizQuestionRequestDTO request);
        Task<ApiResult<bool>> DeleteQuestionAsync(Guid questionId);
    }
}
