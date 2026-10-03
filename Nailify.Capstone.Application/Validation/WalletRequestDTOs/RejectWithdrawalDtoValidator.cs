using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.WalletRequestDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Validation.WalletRequestDTOs
{
    public class RejectWithdrawalDtoValidator : AbstractValidator<RejectWithdrawalDto>
    {
        public RejectWithdrawalDtoValidator()
        {
            RuleFor(x => x.AdminNote)
                .NotEmpty().WithMessage("Vui lòng nhập lý do từ chối yêu cầu rút tiền.");
        }
    }
}
