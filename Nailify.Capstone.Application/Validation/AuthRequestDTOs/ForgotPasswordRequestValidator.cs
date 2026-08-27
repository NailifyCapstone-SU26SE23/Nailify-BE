using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.AuthRequestDTOs;

namespace Nailify.Capstone.Application.Validation.AuthRequestDTOs
{
    public class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
    {
        public ForgotPasswordRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email format is invalid.");
        }
    }
}
