using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailShapeRequestDTOs;

namespace Nailify.Capstone.Application.Validation.NailShapeRequestDTOs
{
    public class NailShapeUpdateRequestValidator : AbstractValidator<NailShapeUpdateRequest>
    {
        public NailShapeUpdateRequestValidator()
        {
            RuleFor(x => x.NailShapeId).GreaterThan(0);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.ImageUrl).MaximumLength(500);
            RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        }
    }
}
