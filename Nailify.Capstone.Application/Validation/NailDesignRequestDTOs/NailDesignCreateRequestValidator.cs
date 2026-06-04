using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailDesignRequestDTOs;

namespace Nailify.Capstone.Application.Validation.NailDesignRequestDTOs
{
    public class NailDesignCreateRequestValidator : AbstractValidator<NailDesignCreateRequest>
    {
        public NailDesignCreateRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tên mẫu nail không được để trống")
                .MaximumLength(200).WithMessage("Tên mẫu nail không được vượt quá 200 ký tự");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Giá phải lớn hơn 0")
                .LessThan(100000000).WithMessage("Giá không được vượt quá 100,000,000 VNĐ");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Mô tả không được vượt quá 500 ký tự");
        }
    }
}
