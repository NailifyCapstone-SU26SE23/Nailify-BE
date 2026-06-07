using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.CategoryTypeRequestDTOs;

namespace Nailify.Capstone.Application.Validation.CategoryTypeRequestDTOs
{
    public class CategoryTypeUpdateRequestValidator : AbstractValidator<CategoryTypeUpdateRequest>
    {
        public CategoryTypeUpdateRequestValidator()
        {
            RuleFor(x => x.CategoryTypeId)
                .GreaterThan(0).WithMessage("ID loại danh mục không hợp lệ");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tên loại danh mục không được để trống")
                .MaximumLength(200).WithMessage("Tên loại danh mục không được vượt quá 200 ký tự");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Trạng thái không được để trống");
        }
    } 
}
