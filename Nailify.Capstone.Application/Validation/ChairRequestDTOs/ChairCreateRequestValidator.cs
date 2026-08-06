using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.ChairRequestDTOs;

namespace Nailify.Capstone.Application.Validation.ChairRequestDTOs
{
    public class ChairCreateRequestValidator : AbstractValidator<ChairCreateRequest>
    {
        public ChairCreateRequestValidator()
        {
            RuleFor(x => x.SalonId)
                .NotEmpty().WithMessage("Vui lòng chọn chi nhánh Salon cho ghế.");

            RuleFor(x => x.ChairName)
                .NotEmpty().WithMessage("Tên ghế không được để trống.")
                .MaximumLength(100).WithMessage("Tên ghế không vượt quá 100 ký tự.");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Trạng thái ghế không được để trống.")
                .Must(status => status == "Active" || status == "Maintenance" || status == "Inactive")
                .WithMessage("Trạng thái ghế phải là 'Active', 'Maintenance' hoặc 'Inactive'.");
        }
    }
}
