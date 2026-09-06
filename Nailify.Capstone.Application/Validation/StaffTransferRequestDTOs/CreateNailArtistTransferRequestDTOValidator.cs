using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.StaffTransferRequestDTOs;
using System;

namespace Nailify.Capstone.Application.Validation.StaffTransferRequestDTOs
{
    public class CreateNailArtistTransferRequestDTOValidator : AbstractValidator<CreateNailArtistTransferRequestDTO>
    {
        public CreateNailArtistTransferRequestDTOValidator()
        {
            RuleFor(x => x.NailArtistId)
                .NotEmpty().WithMessage("Vui lòng chọn thợ móng điều chuyển.");

            RuleFor(x => x.ToSalonId)
                .NotEmpty().WithMessage("Vui lòng chọn chi nhánh Salon đích điều chuyển.");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Vui lòng chọn ngày bắt đầu điều chuyển.")
                .Must(date => date.Date >= DateTime.UtcNow.AddHours(7).Date)
                .WithMessage("Ngày bắt đầu điều chuyển không được ở trong quá khứ.");

            RuleFor(x => x.EndDate)
                .GreaterThanOrEqualTo(x => x.StartDate)
                .WithMessage("Ngày kết thúc điều chuyển (EndDate) phải sau hoặc bằng ngày bắt đầu (StartDate).");

            RuleFor(x => x.Reason)
                .MaximumLength(500).WithMessage("Lý do điều chuyển không vượt quá 500 ký tự.");
        }
    }
}
