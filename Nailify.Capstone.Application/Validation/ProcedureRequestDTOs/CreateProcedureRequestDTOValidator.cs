using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.ProcedureRequestDTOs;

namespace Nailify.Capstone.Application.Validation.ProcedureRequestDTOs
{
    public class CreateProcedureRequestDTOValidator : AbstractValidator<CreateProcedureRequestDTO>
    {
        public CreateProcedureRequestDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tên quy trình bước làm móng không được để trống.")
                .MaximumLength(150).WithMessage("Tên quy trình không vượt quá 150 ký tự.");

            RuleFor(x => x.Duration)
                .GreaterThan(0)
                .When(x => x.Duration.HasValue)
                .WithMessage("Thời lượng ước tính của quy trình phải lớn hơn 0 phút.");
        }
    }
}
