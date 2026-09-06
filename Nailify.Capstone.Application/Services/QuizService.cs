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
using System.Text.Json;
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

        /// <summary>
        /// Lấy toàn bộ câu hỏi Quiz đang active kèm các phương án.
        /// </summary>
        public async Task<ApiResult<List<StyleQuizQuestionResponseDTO>>> GetQuizQuestionsAsync()
        {
            var questions = await _unitOfWork.QuizQuestionRepository.GetActiveQuestionsWithOptionsAsync();
            var result = _mapper.Map<List<StyleQuizQuestionResponseDTO>>(questions);
            return new ApiSuccessResult<List<StyleQuizQuestionResponseDTO>>(result, "Lấy câu hỏi Quiz thành công.");
        }

        /// <summary>
        /// Tạo câu hỏi mới (không có option — thêm option qua AddOptionToQuestionAsync).
        /// </summary>
        public async Task<ApiResult<StyleQuizQuestionResponseDTO>> CreateQuestionAsync(
            QuizQuestionType type, QuizCategory category, CreateQuizQuestionRequestDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.QuestionText))
            {
                return new ApiErrorResult<StyleQuizQuestionResponseDTO>("Nội dung câu hỏi không được để trống.");
            }
            var question = _mapper.Map<QuizQuestion>(request);
            question.Type = type;
            question.Category = category;
            question.IsActive = true;
            await _unitOfWork.QuizQuestionRepository.CreateAsync(question);
            await _unitOfWork.SaveChangesAsync();
            var created = await _unitOfWork.QuizQuestionRepository.GetQuestionWithOptionsAsync(question.QuizQuestionId);
            var response = _mapper.Map<StyleQuizQuestionResponseDTO>(created);
            return new ApiSuccessResult<StyleQuizQuestionResponseDTO>(response, "Tạo câu hỏi Quiz thành công.");
        }

        /// <summary>
        /// Cập nhật nội dung câu hỏi (không đụng tới options — quản lý option qua endpoint riêng).
        /// </summary>
        public async Task<ApiResult<StyleQuizQuestionResponseDTO>> UpdateQuestionAsync(
            Guid questionId, QuizQuestionType type, QuizCategory category, UpdateQuizQuestionRequestDTO request)
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
            question.QuestionText = request.QuestionText;
            question.Type = type;
            question.Category = category;
            question.IsActive = request.IsActive;
            _unitOfWork.QuizQuestionRepository.Update(question);
            await _unitOfWork.SaveChangesAsync();
            var updated = await _unitOfWork.QuizQuestionRepository.GetQuestionWithOptionsAsync(questionId);
            var response = _mapper.Map<StyleQuizQuestionResponseDTO>(updated);
            return new ApiSuccessResult<StyleQuizQuestionResponseDTO>(response, "Cập nhật câu hỏi Quiz thành công.");
        }

        /// <summary>
        /// Xóa một câu hỏi Quiz (cascade xóa cả options).
        /// </summary>
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

        /// <summary>
        /// Thêm một phương án trả lời vào câu hỏi.
        /// optionValues truyền qua query dạng array: ?optionValues=val1&amp;optionValues=val2
        /// </summary>
        public async Task<ApiResult<StyleQuizQuestionResponseDTO>> AddOptionToQuestionAsync(
            Guid questionId, AddQuizOptionRequestDTO request)
        {
            var question = await _unitOfWork.QuizQuestionRepository.GetQuestionWithOptionsAsync(questionId);
            if (question == null)
            {
                return new ApiErrorResult<StyleQuizQuestionResponseDTO>("Không tìm thấy câu hỏi Quiz.");
            }
            if (string.IsNullOrWhiteSpace(request.Label))
            {
                return new ApiErrorResult<StyleQuizQuestionResponseDTO>("Label của phương án không được để trống.");
            }
            var option = new QuizOption
            {
                QuizQuestionId = questionId,
                Label = request.Label,
                Description = request.Description,
                OptionValue = JsonSerializer.Serialize(request.OptionValues)
            };
            await _unitOfWork.QuizOptionRepository.CreateAsync(option);
            await _unitOfWork.SaveChangesAsync();
            var updated = await _unitOfWork.QuizQuestionRepository.GetQuestionWithOptionsAsync(questionId);
            var response = _mapper.Map<StyleQuizQuestionResponseDTO>(updated);
            return new ApiSuccessResult<StyleQuizQuestionResponseDTO>(response, "Thêm phương án trả lời thành công.");
        }

        /// <summary>
        /// Xóa một phương án trả lời khỏi câu hỏi.
        /// </summary>
        public async Task<ApiResult<bool>> DeleteOptionAsync(Guid optionId)
        {
            var option = await _unitOfWork.QuizOptionRepository.GetByIdAsync(optionId);
            if (option == null)
            {
                return new ApiErrorResult<bool>("Không tìm thấy phương án trả lời cần xóa.");
            }
            _unitOfWork.QuizOptionRepository.Delete(option);
            await _unitOfWork.SaveChangesAsync();
            return new ApiSuccessResult<bool>(true, "Xóa phương án trả lời thành công.");
        }
    }
}
