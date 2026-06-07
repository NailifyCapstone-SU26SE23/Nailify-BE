using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.CategoryTypeRequestDTOs;

namespace Nailify.Capstone.Application.Validation.CategoryTypeRequestDTOs
{
    public class CategoryTypeUpdateRequestValidator : AbstractValidator<CategoryTypeUpdateRequest>
    {
        public CategoryTypeUpdateRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Ten loai danh muc khong duoc de trong")
                .MaximumLength(200).WithMessage("Ten loai danh muc khong duoc vuot qua 200 ky tu");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Trang thai khong duoc de trong");
        }
    }
}
