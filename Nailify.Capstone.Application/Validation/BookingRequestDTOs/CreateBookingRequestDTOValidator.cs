using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs;
using System;

namespace Nailify.Capstone.Application.Validation.BookingRequestDTOs
{
    public class CreateBookingRequestDTOValidator : AbstractValidator<CreateBookingRequestDTO>
    {
        public CreateBookingRequestDTOValidator()
        {
            RuleFor(x => x.SalonId)
                .NotEmpty().WithMessage("Vui lòng chọn chi nhánh Salon.");

            RuleFor(x => x.BookingDate)
                .NotEmpty().WithMessage("Vui lòng chọn ngày đặt lịch.")
                .Must(date => date.Date >= DateTime.UtcNow.AddHours(7).Date)
                .WithMessage("Ngày đặt lịch không được là ngày trong quá khứ.");

            RuleFor(x => x.StartTime)
                .NotEmpty().WithMessage("Vui lòng chọn giờ bắt đầu.");

            RuleFor(x => x.BookingItems)
                .NotEmpty().WithMessage("Đơn đặt lịch phải có ít nhất 1 dịch vụ hoặc mẫu móng.")
                .Must(items => items != null && items.Count > 0)
                .WithMessage("Danh sách dịch vụ không được trống.");

            RuleForEach(x => x.BookingItems).ChildRules(item =>
            {
                item.RuleFor(i => i)
                    .Must(i => i.ServiceId.HasValue || i.NailVariantId.HasValue || i.CustomerNailId.HasValue)
                    .WithMessage("Mỗi mục đặt lịch phải chứa Dịch vụ (Service), Biến thể móng (NailVariant) hoặc Mẫu thiết kế riêng (CustomerNail).");
            });
        }
    }
}
