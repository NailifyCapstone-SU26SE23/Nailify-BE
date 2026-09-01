using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.SalonRequestDTOs;

namespace Nailify.Capstone.Application.Validation.SalonRequestDTOs
{
    public class SalonUpdateRequestValidator : AbstractValidator<SalonUpdateRequest>
    {
        public SalonUpdateRequestValidator()
        {
            RuleFor(x => x.Name)
                .MaximumLength(200).WithMessage("Tên chi nhánh Salon không vượt quá 200 ký tự.")
                .When(x => !string.IsNullOrEmpty(x.Name));

            RuleFor(x => x.Phone)
                .Matches(@"^(0[3|5|7|8|9])+([0-9]{8})$")
                .When(x => !string.IsNullOrEmpty(x.Phone))
                .WithMessage("Số điện thoại Salon không hợp lệ.");

            RuleFor(x => x.Status)
                .Must(status => string.IsNullOrEmpty(status) || status == "Open" || status == "Closed")
                .WithMessage("Trạng thái Salon phải là 'Open' hoặc 'Closed'.");
        }
    }
}
