using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.UserRequestDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Validation.UserRequestDTOs
{
    public class UserRegisterRequestValidator : AbstractValidator<UserRegisterRequest>
    {
        public UserRegisterRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email không được để trống.")
                .EmailAddress().WithMessage("Email không đúng định dạng.");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Mật khẩu không được để trống.")
                .MinimumLength(6).WithMessage("Mật khẩu phải chứa ít nhất 6 ký tự.");
            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Mật khẩu xác nhận không được để trống.")
                .Equal(x => x.Password).WithMessage("Mật khẩu xác nhận không trùng khớp.");
            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Số điện thoại không được để trống.")
                .Matches(@"^(0[3|5|7|8|9])([0-9]{8})$").WithMessage("Số điện thoại Việt Nam không hợp lệ.");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Tên không được để trống.");
        }
    }
}

