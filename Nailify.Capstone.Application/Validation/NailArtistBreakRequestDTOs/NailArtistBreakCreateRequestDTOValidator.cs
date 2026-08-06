using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailArtistBreakRequestDTOs;
using System;

namespace Nailify.Capstone.Application.Validation.NailArtistBreakRequestDTOs
{
    public class NailArtistBreakCreateRequestDTOValidator : AbstractValidator<NailArtistBreakCreateRequestDTO>
    {
        public NailArtistBreakCreateRequestDTOValidator()
        {
            RuleFor(x => x.NailArtistId)
                .NotEmpty().WithMessage("Vui lòng chọn thợ móng xin nghỉ giữa ca.");

            RuleFor(x => x.BreakDate)
                .NotEmpty().WithMessage("Vui lòng chọn ngày xin nghỉ.")
                .Must(date => date.Date >= DateTime.UtcNow.AddHours(7).Date)
                .WithMessage("Ngày xin nghỉ không được ở trong quá khứ.");

            RuleFor(x => x.StartTime)
                .NotEmpty().WithMessage("Vui lòng nhập giờ bắt đầu nghỉ (HH:mm).")
                .Must(BeValidTimeSpan).WithMessage("Định dạng giờ bắt đầu nghỉ không hợp lệ (HH:mm).");

            RuleFor(x => x.EndTime)
                .NotEmpty().WithMessage("Vui lòng nhập giờ kết thúc nghỉ (HH:mm).")
                .Must(BeValidTimeSpan).WithMessage("Định dạng giờ kết thúc nghỉ không hợp lệ (HH:mm).");

            RuleFor(x => x.Reason)
                .MaximumLength(255).WithMessage("Lý do nghỉ không vượt quá 255 ký tự.");
        }

        private bool BeValidTimeSpan(string timeStr)
        {
            return TimeSpan.TryParse(timeStr, out _);
        }
    }
}
