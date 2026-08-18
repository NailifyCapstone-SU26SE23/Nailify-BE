using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailShapeRequestDTOs;

namespace Nailify.Capstone.Application.Validation.NailShapeRequestDTOs
{
    public class NailShapeCreateRequestValidator : AbstractValidator<NailShapeCreateRequest>
    {
        public NailShapeCreateRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        }
    }
}
