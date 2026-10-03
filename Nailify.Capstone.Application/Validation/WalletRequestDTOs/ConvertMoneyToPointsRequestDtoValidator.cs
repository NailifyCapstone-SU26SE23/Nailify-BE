using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.WalletRequestDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Validation.WalletRequestDTOs
{
    public class ConvertMoneyToPointsRequestDtoValidator : AbstractValidator<ConvertMoneyToPointsRequestDto>
    {
        public ConvertMoneyToPointsRequestDtoValidator()
        {
            RuleFor(x => x.MoneyAmount)
                .GreaterThanOrEqualTo(10000m).WithMessage("Số tiền quy đổi tối thiểu phải là 10,000 VND.")
                .Must(amount => amount % 10000m == 0).WithMessage("Số tiền quy đổi phải là bội số của 10,000 VND.");
        }
    }
}
