using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailShapeRequestDTOs;

namespace Nailify.Capstone.Application.Validation.NailShapeRequestDTOs
{
    public class NailShapeUpdateRequestValidator : AbstractValidator<NailShapeUpdateRequest>
    {
        public NailShapeUpdateRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        }
    }
}
