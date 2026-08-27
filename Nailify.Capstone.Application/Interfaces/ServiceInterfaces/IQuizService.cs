using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.QuizRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.QuizResponseDTOs;
using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface IQuizService
    {
        Task<ApiResult<List<StyleQuizQuestionResponseDTO>>> GetQuizQuestionsAsync();
        Task<ApiResult<StyleQuizQuestionResponseDTO>> CreateQuestionAsync(QuizQuestionType type, QuizCategory category, CreateQuizQuestionRequestDTO request);
        Task<ApiResult<StyleQuizQuestionResponseDTO>> UpdateQuestionAsync(Guid questionId, QuizQuestionType type, QuizCategory category, UpdateQuizQuestionRequestDTO request);
        Task<ApiResult<bool>> DeleteQuestionAsync(Guid questionId);
        Task<ApiResult<StyleQuizQuestionResponseDTO>> AddOptionToQuestionAsync(Guid questionId, AddQuizOptionRequestDTO request);
        Task<ApiResult<bool>> DeleteOptionAsync(Guid optionId);
    }
}
