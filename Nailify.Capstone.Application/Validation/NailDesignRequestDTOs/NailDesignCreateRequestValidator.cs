using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailDesignRequestDTOs;

namespace Nailify.Capstone.Application.Validation.NailDesignRequestDTOs
{
    public class NailDesignCreateRequestValidator : AbstractValidator<NailDesignCreateRequest>
    {
        public NailDesignCreateRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Ten mau nail khong duoc de trong")
                .MaximumLength(200).WithMessage("Ten mau nail khong duoc vuot qua 200 ky tu");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Gia phai lon hon 0")
                .LessThan(100000000).WithMessage("Gia khong duoc vuot qua 100,000,000 VND");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Mo ta khong duoc vuot qua 500 ky tu");

            RuleForEach(x => x.NailVariantIds)
                .GreaterThan(0).WithMessage("ID bien the mong khong hop le");
        }
    }
}
