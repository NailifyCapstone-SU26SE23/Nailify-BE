using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.AuthRequestDTOs;

namespace Nailify.Capstone.Application.Validation.AuthRequestDTOs
{
    public class CheckResetPasswordTokenRequestValidator : AbstractValidator<CheckResetPasswordTokenRequest>
    {
        public CheckResetPasswordTokenRequestValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Reset token is required.");
        }
    }
}
