using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.CustomerComponentRequestDTOs;

namespace Nailify.Capstone.Application.Validation.CustomerComponentRequestDTOs
{
    public class CustomerComponentCreateRequestValidator : AbstractValidator<CustomerComponentCreateRequest>
    {
        public CustomerComponentCreateRequestValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Price).GreaterThanOrEqualTo(0).When(x => x.Price.HasValue);
        }
    }
}
