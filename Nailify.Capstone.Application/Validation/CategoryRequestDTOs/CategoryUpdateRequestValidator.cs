using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.CategoryRequestDTOs;

namespace Nailify.Capstone.Application.Validation.CategoryRequestDTOs
{
    public class CategoryUpdateRequestValidator : AbstractValidator<CategoryUpdateRequest>
    {
        public CategoryUpdateRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Ten danh muc khong duoc de trong")
                .MaximumLength(200).WithMessage("Ten danh muc khong duoc vuot qua 200 ky tu");

            RuleFor(x => x.CategoryTypeId)
                .GreaterThan(0).WithMessage("ID loai danh muc khong hop le");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Trang thai khong duoc de trong");
        }
    }
}
