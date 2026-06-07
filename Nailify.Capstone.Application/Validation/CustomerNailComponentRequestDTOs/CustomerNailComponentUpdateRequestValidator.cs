using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.CustomerNailComponentRequestDTOs;

namespace Nailify.Capstone.Application.Validation.CustomerNailComponentRequestDTOs
{
    public class CustomerNailComponentUpdateRequestValidator : AbstractValidator<CustomerNailComponentUpdateRequest>
    {
        public CustomerNailComponentUpdateRequestValidator()
        {
            RuleFor(x => x.CustomerNailId).GreaterThan(0);
            RuleFor(x => x.ComponentId).GreaterThan(0).When(x => x.ComponentId.HasValue);
            RuleFor(x => x.CustomerComponentId).GreaterThan(0).When(x => x.CustomerComponentId.HasValue);
            RuleFor(x => x)
                .Must(x => x.ComponentId.HasValue != x.CustomerComponentId.HasValue)
                .WithMessage("Chi duoc chon mot trong Component hoac CustomerComponent.");
        }
    }
}
