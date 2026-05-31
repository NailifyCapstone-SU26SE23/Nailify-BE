using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.CategoryRequestDTOs;

namespace Nailify.Capstone.Application.Validation.CategoryRequestDTOs
{
    public class CategoryUpdateRequestValidator : AbstractValidator<CategoryUpdateRequest>
    {
        public CategoryUpdateRequestValidator()
        {
            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("ID danh mục không hợp lệ");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tên danh mục không được để trống")
                .MaximumLength(200).WithMessage("Tên danh mục không được vượt quá 200 ký tự");

            RuleFor(x => x.CategoryTypeId)
                .GreaterThan(0).WithMessage("ID loại danh mục không hợp lệ");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Trạng thái không được để trống");
        }
    }
}
