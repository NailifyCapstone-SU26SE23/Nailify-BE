using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.QuizRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.QuizResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Services
{
    public class QuizService : IQuizService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public QuizService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResult<StyleQuizQuestionResponseDTO>> CreateQuestionAsync(CreateQuizQuestionRequestDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.QuestionText))
            {
                return new ApiErrorResult<StyleQuizQuestionResponseDTO>("Nội dung câu hỏi không được để trống.");
            }
            if (!Enum.TryParse<QuizQuestionType>(request.Type, true, out var questionType))
            {
                return new ApiErrorResult<StyleQuizQuestionResponseDTO>("Loại câu hỏi không hợp lệ (hỗ trợ 'single' hoặc 'multiple').");
            }
            if (!Enum.TryParse<QuizCategory>(request.Category, true, out var quizCategory))
            {
                return new ApiErrorResult<StyleQuizQuestionResponseDTO>("Category câu hỏi không hợp lệ.");
            }
            var question = _mapper.Map<QuizQuestion>(request);
            question.IsActive = true;
            await _unitOfWork.QuizQuestionRepository.CreateAsync(question);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<StyleQuizQuestionResponseDTO>(question);
            return new ApiSuccessResult<StyleQuizQuestionResponseDTO>(response, "Tạo câu hỏi Quiz thành công.");
        }

        public async Task<ApiResult<bool>> DeleteQuestionAsync(Guid questionId)
        {
            var question = await _unitOfWork.QuizQuestionRepository.GetByIdAsync(questionId);
            if (question == null)
            {
                return new ApiErrorResult<bool>("Không tìm thấy câu hỏi Quiz cần xóa.");
            }
            _unitOfWork.QuizQuestionRepository.Delete(question);
            await _unitOfWork.SaveChangesAsync();
            return new ApiSuccessResult<bool>(true, "Xóa câu hỏi Quiz thành công.");
        }

        public async Task<ApiResult<List<StyleQuizQuestionResponseDTO>>> GetQuizQuestionsAsync()
        {
            var questions = await _unitOfWork.QuizQuestionRepository.GetActiveQuestionsWithOptionsAsync();
            var result = _mapper.Map<List<StyleQuizQuestionResponseDTO>>(questions);
            return new ApiSuccessResult<List<StyleQuizQuestionResponseDTO>>(result, "Lấy câu hỏi Quiz thành công.");
        }

        public async Task<ApiResult<StyleQuizQuestionResponseDTO>> UpdateQuestionAsync(Guid questionId, UpdateQuizQuestionRequestDTO request)
        {
            var question = await _unitOfWork.QuizQuestionRepository.GetQuestionWithOptionsAsync(questionId);
            if (question == null)
            {
                return new ApiErrorResult<StyleQuizQuestionResponseDTO>("Không tìm thấy câu hỏi Quiz cần cập nhật.");
            }
            if (string.IsNullOrWhiteSpace(request.QuestionText))
            {
                return new ApiErrorResult<StyleQuizQuestionResponseDTO>("Nội dung câu hỏi không được để trống.");
            }
            if (!Enum.TryParse<QuizQuestionType>(request.Type, true, out var questionType))
            {
                return new ApiErrorResult<StyleQuizQuestionResponseDTO>("Loại câu hỏi không hợp lệ.");
            }
            if (!Enum.TryParse<QuizCategory>(request.Category, true, out var quizCategory))
            {
                return new ApiErrorResult<StyleQuizQuestionResponseDTO>("Category câu hỏi không hợp lệ.");
            }
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                question.QuestionText = request.QuestionText;
                question.Type = questionType;
                question.Category = quizCategory;
                question.IsActive = request.IsActive;
                foreach (var opt in question.QuizOptions.ToList())
                {
                    _unitOfWork.QuizOptionRepository.Delete(opt);
                }
                await _unitOfWork.SaveChangesAsync();
                foreach (var optDto in request.Options)
                {
                    var newOpt = _mapper.Map<QuizOption>(optDto);
                    newOpt.QuizQuestionId = questionId;
                    await _unitOfWork.QuizOptionRepository.CreateAsync(newOpt);
                }
                _unitOfWork.QuizQuestionRepository.Update(question);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
                var updatedQuestion = await _unitOfWork.QuizQuestionRepository.GetQuestionWithOptionsAsync(questionId);
                var response = _mapper.Map<StyleQuizQuestionResponseDTO>(updatedQuestion);
                return new ApiSuccessResult<StyleQuizQuestionResponseDTO>(response, "Cập nhật câu hỏi Quiz thành công.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new ApiErrorResult<StyleQuizQuestionResponseDTO>($"Lỗi khi cập nhật câu hỏi: {ex.Message}");
            }
        }
    }
}
