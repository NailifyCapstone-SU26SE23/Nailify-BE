using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailComponentRequestDTOs;

namespace Nailify.Capstone.Application.Validation.NailComponentRequestDTOs
{
    public class NailComponentUpdateRequestValidator : AbstractValidator<NailComponentUpdateRequest>
    {
        public NailComponentUpdateRequestValidator()
        {
            RuleFor(x => x.NailComponentId).GreaterThan(0);
            RuleFor(x => x.ComponentId).GreaterThan(0);
            RuleFor(x => x.NailVariantId).GreaterThan(0);
            RuleFor(x => x.FingerIndex).InclusiveBetween(-1, 9);
            RuleFor(x => x.ConfigJson).MaximumLength(4000);
        }
    }
}
