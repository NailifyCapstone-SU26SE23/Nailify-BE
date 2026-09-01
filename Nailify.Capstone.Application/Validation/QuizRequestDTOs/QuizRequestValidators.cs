using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.QuizRequestDTOs;

namespace Nailify.Capstone.Application.Validation.QuizRequestDTOs
{
    public class CreateQuizQuestionRequestDTOValidator : AbstractValidator<CreateQuizQuestionRequestDTO>
    {
        public CreateQuizQuestionRequestDTOValidator()
        {
            RuleFor(x => x.QuestionText)
                .NotEmpty().WithMessage("Nội dung câu hỏi Quiz tư vấn móng không được để trống.")
                .MaximumLength(500).WithMessage("Nội dung câu hỏi không vượt quá 500 ký tự.");
        }
    }

    public class CreateQuizOptionRequestDTOValidator : AbstractValidator<CreateQuizOptionRequestDTO>
    {
        public CreateQuizOptionRequestDTOValidator()
        {
            RuleFor(x => x.Label)
                .NotEmpty().WithMessage("Nhãn lựa chọn (Label) không được để trống.")
                .MaximumLength(200).WithMessage("Nhãn lựa chọn không vượt quá 200 ký tự.");

            RuleFor(x => x.OptionValues)
                .NotEmpty().WithMessage("Danh sách giá trị lựa chọn (OptionValues) không được trống.");
        }
    }

    public class SubmitQuizAnswersRequestDtoValidator : AbstractValidator<SubmitQuizAnswersRequestDto>
    {
        public SubmitQuizAnswersRequestDtoValidator()
        {
            RuleFor(x => x.SelectedOptionIds)
                .NotEmpty().WithMessage("Khách hàng phải chọn ít nhất 1 phương án trả lời khi nộp Quiz tư vấn móng.");
        }
    }
}
