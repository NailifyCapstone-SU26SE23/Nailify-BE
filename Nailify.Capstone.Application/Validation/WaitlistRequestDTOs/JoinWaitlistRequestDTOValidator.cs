using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.WaitlistRequestDTOs;
using System;

namespace Nailify.Capstone.Application.Validation.WaitlistRequestDTOs
{
    public class JoinWaitlistRequestDTOValidator : AbstractValidator<JoinWaitlistRequestDTO>
    {
        public JoinWaitlistRequestDTOValidator()
        {
            RuleFor(x => x.SalonId)
                .NotEmpty().WithMessage("Vui lòng chọn chi nhánh Salon để đăng ký hàng chờ (Waitlist).");

            RuleFor(x => x.RequestedDate)
                .NotEmpty().WithMessage("Vui lòng chọn ngày mong muốn xếp hàng chờ.")
                .Must(date => date.Date >= DateTime.UtcNow.AddHours(7).Date)
                .WithMessage("Ngày xếp hàng chờ không được là ngày trong quá khứ.");

            RuleFor(x => x.RequestedStartTime)
                .NotEmpty().WithMessage("Vui lòng chọn khung giờ mong muốn.");

            RuleFor(x => x.WaitlistItems)
                .NotEmpty().WithMessage("Đăng ký danh sách chờ phải có ít nhất 1 dịch vụ hoặc mẫu móng.");
        }
    }
}
