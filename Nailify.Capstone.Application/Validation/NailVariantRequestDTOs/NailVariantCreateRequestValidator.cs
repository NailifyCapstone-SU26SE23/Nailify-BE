using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailVariantRequestDTOs;

namespace Nailify.Capstone.Application.Validation.NailVariantRequestDTOs
{
    public class NailVariantCreateRequestValidator : AbstractValidator<NailVariantCreateRequest>
    {
        public NailVariantCreateRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.NailShapeId).GreaterThan(0);
            RuleFor(x => x.NailSurfaceId).GreaterThan(0);
            RuleFor(x => x.NailDesignId).GreaterThan(0);
        }
    }
}
