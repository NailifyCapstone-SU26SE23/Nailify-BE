using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailSurfaceRequestDTOs;

namespace Nailify.Capstone.Application.Validation.NailSurfaceRequestDTOs
{
    public class NailSurfaceUpdateRequestValidator : AbstractValidator<NailSurfaceUpdateRequest>
    {
        public NailSurfaceUpdateRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.ShaderParam).MaximumLength(1000);
            RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        }
    }
}
