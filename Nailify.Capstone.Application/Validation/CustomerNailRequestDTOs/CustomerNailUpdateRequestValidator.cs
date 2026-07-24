using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.CustomerNailRequestDTOs;

namespace Nailify.Capstone.Application.Validation.CustomerNailRequestDTOs
{
    public class CustomerNailUpdateRequestValidator : AbstractValidator<CustomerNailUpdateRequest>
    {
        public CustomerNailUpdateRequestValidator()
        {
            RuleFor(x => x.Name)
                .MaximumLength(200)
                .When(x => !string.IsNullOrWhiteSpace(x.Name));
        }
    }
}
