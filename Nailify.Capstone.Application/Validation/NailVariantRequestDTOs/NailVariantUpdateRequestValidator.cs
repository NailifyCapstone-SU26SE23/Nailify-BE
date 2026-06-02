using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailVariantRequestDTOs;

namespace Nailify.Capstone.Application.Validation.NailVariantRequestDTOs
{
    public class NailVariantUpdateRequestValidator : AbstractValidator<NailVariantUpdateRequest>
    {
        public NailVariantUpdateRequestValidator()
        {
            RuleFor(x => x.NailVariantId).GreaterThan(0);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.NailShapeId).GreaterThan(0);
            RuleFor(x => x.NailSurfaceId).GreaterThan(0);
            RuleFor(x => x.NailDesignId).GreaterThan(0);
            RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Color).MaximumLength(100);
            RuleFor(x => x.Form).MaximumLength(100);
            RuleFor(x => x.Material).MaximumLength(100);
        }
    }
}
