using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.UserRequestDTOs;

namespace Nailify.Capstone.Application.Validation.UserRequestDTOs
{
    public class UserCreateRequestValidator : AbstractValidator<UserCreateRequest>
    {
        public UserCreateRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email không được để trống.")
                .EmailAddress().WithMessage("Email không đúng định dạng.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Mật khẩu không được để trống.")
                .MinimumLength(6).WithMessage("Mật khẩu phải chứa ít nhất 6 ký tự.");

            RuleFor(x => x.Phone)
                .Matches(@"^(0[3|5|7|8|9])([0-9]{8})$").WithMessage("Số điện thoại Việt Nam không hợp lệ.")
                .When(x => !string.IsNullOrEmpty(x.Phone));

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Tên không được để trống.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Họ không được để trống.");
        }
    }
}
