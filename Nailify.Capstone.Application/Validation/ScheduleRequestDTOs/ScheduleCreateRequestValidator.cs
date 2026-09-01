using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.ScheduleRequestDTOs;

namespace Nailify.Capstone.Application.Validation.ScheduleRequestDTOs
{
    public class ScheduleCreateRequestValidator : AbstractValidator<ScheduleCreateRequest>
    {
        public ScheduleCreateRequestValidator()
        {
            RuleFor(x => x.NailArtistId)
                .NotEmpty().WithMessage("Vui lòng chọn thợ móng để xếp lịch làm việc.");

            RuleFor(x => x.WorkDate)
                .NotEmpty().WithMessage("Vui lòng chọn ngày làm việc.")
                .Must(date => date.Date >= System.DateTime.UtcNow.AddHours(7).Date)
                .WithMessage("Ngày làm việc không được xếp trong quá khứ.");

            RuleFor(x => x.ShiftEnd)
                .GreaterThan(x => x.ShiftStart)
                .WithMessage("Giờ kết thúc ca làm (ShiftEnd) phải sau giờ bắt đầu (ShiftStart).");
        }
    }
}
