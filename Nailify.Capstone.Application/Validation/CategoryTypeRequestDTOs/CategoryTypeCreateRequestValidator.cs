using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.CategoryTypeRequestDTOs;

namespace Nailify.Capstone.Application.Validation.CategoryTypeRequestDTOs
{
    public class CategoryTypeCreateRequestValidator : AbstractValidator<CategoryTypeCreateRequest>
    {
        public CategoryTypeCreateRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tên loại danh mục không được để trống")
                .MaximumLength(200).WithMessage("Tên loại danh mục không được vượt quá 200 ký tự");
        }
    }
}
