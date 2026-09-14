using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.WalletRequestDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Validation.WalletRequestDTOs
{
    public class ApproveWithdrawalDtoValidator : AbstractValidator<ApproveWithdrawalDto>
    {
        public ApproveWithdrawalDtoValidator()
        {
            RuleFor(x => x.TransactionReference)
                .NotEmpty()
                .WithMessage("Mã giao dịch đối soát ngân hàng không được để trống.");
        }
    }
}
