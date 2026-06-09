using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.UserRequestDTOs;

namespace Nailify.Capstone.Application.Validation.UserRequestDTOs
{
    public class CustomerPreferencesUpdateRequestValidator : AbstractValidator<CustomerPreferencesUpdateRequest>
    {
        public CustomerPreferencesUpdateRequestValidator()
        {
            RuleFor(x => x.SkinTone)
                .NotEmpty().WithMessage("Màu da (SkinTone) không được để trống.");

            RuleFor(x => x.Occupation)
                .NotEmpty().WithMessage("Nghề nghiệp (Occupation) không được để trống.");

            RuleFor(x => x.NailCondition)
                .NotEmpty().WithMessage("Tình trạng móng (NailCondition) không được để trống.");

        }
    }
}