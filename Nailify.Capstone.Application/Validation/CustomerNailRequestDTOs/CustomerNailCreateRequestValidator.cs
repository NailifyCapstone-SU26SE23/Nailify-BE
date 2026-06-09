using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.CustomerNailRequestDTOs;

namespace Nailify.Capstone.Application.Validation.CustomerNailRequestDTOs
{
    public class CustomerNailCreateRequestValidator : AbstractValidator<CustomerNailCreateRequest>
    {
        public CustomerNailCreateRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        }
    }
}
