using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailShapeRequestDTOs;

namespace Nailify.Capstone.Application.Validation.NailShapeRequestDTOs
{
    public class NailShapeUpdateRequestValidator : AbstractValidator<NailShapeUpdateRequest>
    {
        public NailShapeUpdateRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Duration).GreaterThanOrEqualTo(0).When(x => x.Duration.HasValue);
        }
    }
}
