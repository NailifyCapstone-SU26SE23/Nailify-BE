using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailComponentRequestDTOs;

namespace Nailify.Capstone.Application.Validation.NailComponentRequestDTOs
{
    public class NailComponentCreateRequestValidator : AbstractValidator<NailComponentCreateRequest>
    {
        public NailComponentCreateRequestValidator()
        {
            RuleFor(x => x.ComponentId).GreaterThan(0);
            RuleFor(x => x.NailVariantId).GreaterThan(0);
            RuleFor(x => x.FingerIndex).InclusiveBetween(-1, 9);
            RuleFor(x => x.ConfigJson).MaximumLength(4000);
        }
    }
}
