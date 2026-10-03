using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.WalletRequestDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Validation.WalletRequestDTOs
{
    public class CreateWithdrawalRequestDtoValidator : AbstractValidator<CreateWithdrawalRequestDto>
    {
        public CreateWithdrawalRequestDtoValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThanOrEqualTo(50000m)
                .WithMessage("Số tiền rút tối thiểu là 50,000 VND.");
            RuleFor(x => x.BankCode)
                .NotEmpty()
                .WithMessage("Mã viết tắt ngân hàng không được để trống.");
            RuleFor(x => x.BankName)
                .NotEmpty()
                .WithMessage("Tên ngân hàng không được để trống.");
            RuleFor(x => x.AccountNumber)
                .NotEmpty()
                .WithMessage("Số tài khoản ngân hàng không được để trống.");
            RuleFor(x => x.AccountHolderName)
                .NotEmpty()
                .WithMessage("Tên chủ tài khoản không được để trống.");
        }
    }
}
