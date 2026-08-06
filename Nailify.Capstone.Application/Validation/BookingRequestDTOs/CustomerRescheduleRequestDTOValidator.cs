using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs;
using System;

namespace Nailify.Capstone.Application.Validation.BookingRequestDTOs
{
    public class CustomerRescheduleRequestDTOValidator : AbstractValidator<CustomerRescheduleRequestDTO>
    {
        public CustomerRescheduleRequestDTOValidator()
        {
            RuleFor(x => x.NewDate)
                .NotEmpty().WithMessage("Vui lòng chọn ngày mới để đổi lịch.")
                .Must(date => date.Date >= DateTime.UtcNow.AddHours(7).Date)
                .WithMessage("Ngày mới không được là ngày trong quá khứ.");

            RuleFor(x => x.NewTime)
                .NotEmpty().WithMessage("Vui lòng chọn giờ mới.");

            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("Vui lòng nhập lý do đổi lịch hẹn.")
                .MinimumLength(3).WithMessage("Lý do đổi lịch quá ngắn.");
        }
    }
}
