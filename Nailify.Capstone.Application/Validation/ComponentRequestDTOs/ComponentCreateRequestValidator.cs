using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.ComponentRequestDTOs;

namespace Nailify.Capstone.Application.Validation.ComponentRequestDTOs
{
    public class ComponentCreateRequestValidator : AbstractValidator<ComponentCreateRequest>
    {
        public ComponentCreateRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
            RuleFor(x => x.ComponentType).IsInEnum();
        }
    }
}
