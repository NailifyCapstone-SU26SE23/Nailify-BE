using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.WalkInQueueRequestDTOs;

namespace Nailify.Capstone.Application.Validation.WalkInQueueRequestDTOs
{
    public class AddToQueueRequestDTOValidator : AbstractValidator<AddToQueueRequestDTO>
    {
        public AddToQueueRequestDTOValidator()
        {
            RuleFor(x => x.SalonId)
                .NotEmpty().WithMessage("Vui lòng chọn chi nhánh Salon để lấy số vãng lai (Walk-in Queue).");

            RuleFor(x => x)
                .Must(x => x.CustomerId.HasValue || !string.IsNullOrWhiteSpace(x.GuestName))
                .WithMessage("Vui lòng cung cấp thông tin tài khoản khách hoặc Tên khách vãng lai.");

            RuleFor(x => x.GuestPhone)
                .Matches(@"^(0[3|5|7|8|9])+([0-9]{8})$")
                .When(x => !string.IsNullOrWhiteSpace(x.GuestPhone))
                .WithMessage("Số điện thoại khách vãng lai không hợp lệ (phải đúng định dạng SĐT Việt Nam).");

            RuleFor(x => x.BookingItems)
                .NotEmpty().WithMessage("Khách vãng lai lấy số phải chọn ít nhất 1 dịch vụ.");
        }
    }
}
