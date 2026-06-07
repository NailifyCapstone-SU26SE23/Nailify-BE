using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.CustomerNailRequestDTOs;

namespace Nailify.Capstone.Application.Validation.CustomerNailRequestDTOs
{
    public class CustomerNailUpdateRequestValidator : AbstractValidator<CustomerNailUpdateRequest>
    {
        public CustomerNailUpdateRequestValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.NailShapeId).GreaterThan(0);
            RuleFor(x => x.NailSurfaceId).GreaterThan(0);
            RuleFor(x => x.BasedOnNailVariantId).GreaterThan(0).When(x => x.BasedOnNailVariantId.HasValue);
        }
    }
}
