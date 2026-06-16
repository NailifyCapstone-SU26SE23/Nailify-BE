using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailSurfaceRequestDTOs;

namespace Nailify.Capstone.Application.Validation.NailSurfaceRequestDTOs
{
    public class NailSurfaceCreateRequestValidator : AbstractValidator<NailSurfaceCreateRequest>
    {
        public NailSurfaceCreateRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.ShaderParam).MaximumLength(1000);
            RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Duration).GreaterThanOrEqualTo(0).When(x => x.Duration.HasValue);
        }
    }
}
