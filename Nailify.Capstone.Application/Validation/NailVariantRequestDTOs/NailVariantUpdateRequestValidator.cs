using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailVariantRequestDTOs;

namespace Nailify.Capstone.Application.Validation.NailVariantRequestDTOs
{
    public class NailVariantUpdateRequestValidator : AbstractValidator<NailVariantUpdateRequest>
    {
        public NailVariantUpdateRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.NailShapeId).GreaterThan(0).When(x => x.NailShapeId.HasValue);
            RuleFor(x => x.NailSurfaceId).GreaterThan(0).When(x => x.NailSurfaceId.HasValue);
            RuleFor(x => x.NailDesignId).GreaterThan(0);
        }
    }
}
