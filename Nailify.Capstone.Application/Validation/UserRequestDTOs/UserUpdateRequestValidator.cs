using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.UserRequestDTOs;

namespace Nailify.Capstone.Application.Validation.UserRequestDTOs
{
    public class UserUpdateRequestValidator : AbstractValidator<UserUpdateRequest>
    {
        public UserUpdateRequestValidator()
        {
            RuleFor(x => x.Phone)
                .Matches(@"^(0[3|5|7|8|9])([0-9]{8})$").WithMessage("Số điện thoại Việt Nam không hợp lệ.")
                .When(x => !string.IsNullOrEmpty(x.Phone));

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Tên không được để trống.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Họ không được để trống.");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Trạng thái hoạt động không được để trống.");
        }
    }
}
