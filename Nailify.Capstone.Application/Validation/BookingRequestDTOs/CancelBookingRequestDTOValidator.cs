using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs;

namespace Nailify.Capstone.Application.Validation.BookingRequestDTOs
{
    public class CancelBookingRequestDTOValidator : AbstractValidator<CancelBookingRequestDTO>
    {
        public CancelBookingRequestDTOValidator()
        {
            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("Vui lòng nhập lý do hủy lịch hẹn.")
                .MinimumLength(5).WithMessage("Lý do hủy lịch phải có ít nhất 5 ký tự.")
                .MaximumLength(500).WithMessage("Lý do hủy không được vượt quá 500 ký tự.");
        }
    }
}
