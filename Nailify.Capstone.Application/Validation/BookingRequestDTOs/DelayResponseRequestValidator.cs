using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs;
using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Validation.BookingRequestDTOs
{
    public class DelayResponseRequestValidator : AbstractValidator<DelayResponseRequest>
    {
        public DelayResponseRequestValidator()
        {
            RuleFor(x => x.CustomerDecision)
             .IsInEnum().WithMessage("Lựa chọn không hợp lệ (chấp nhận Wait, Reassign, hoặc Reschedule).");
            When(x => x.CustomerDecision == DelayCustomerDecision.Reschedule && x.NewDate.HasValue, () =>
            {
                RuleFor(x => x.NewTime)
                    .NotNull().WithMessage("Vui lòng chọn giờ hẹn mới khi chọn ngày dời lịch.");
            });
        }
    }
}
