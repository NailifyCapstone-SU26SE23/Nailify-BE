using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.ComponentRequestDTOs;

namespace Nailify.Capstone.Application.Validation.ComponentRequestDTOs
{
    public class ComponentUpdateRequestValidator : AbstractValidator<ComponentUpdateRequest>
    {
        public ComponentUpdateRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.ImageUrl).MaximumLength(500);
            RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
            RuleFor(x => x.ComponentType).IsInEnum();
        }
    }
}
