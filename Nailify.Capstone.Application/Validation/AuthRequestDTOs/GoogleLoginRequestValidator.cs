using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.AuthRequestDTOs;

namespace Nailify.Capstone.Application.Validation.AuthRequestDTOs
{
    public class GoogleLoginRequestValidator : AbstractValidator<GoogleLoginRequest>
    {
        public GoogleLoginRequestValidator()
        {
            RuleFor(x => x.IdToken)
                .NotEmpty().WithMessage("Google ID token is required.");
        }
    }
}
