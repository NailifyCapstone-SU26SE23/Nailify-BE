using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.SalonRequestDTOs;

namespace Nailify.Capstone.Application.Validation.SalonRequestDTOs
{
    public class SalonCreateRequestValidator : AbstractValidator<SalonCreateRequest>
    {
        public SalonCreateRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tên chi nhánh Salon không được để trống.")
                .MaximumLength(200).WithMessage("Tên chi nhánh không được vượt quá 200 ký tự.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Địa chỉ chi nhánh không được để trống.");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Số điện thoại Salon không được để trống.")
                .Matches(@"^(0[3|5|7|8|9])+([0-9]{8})$")
                .WithMessage("Số điện thoại Salon không hợp lệ (phải đúng định dạng SĐT Việt Nam).");

            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90).WithMessage("Tọa độ Vĩ độ (Latitude) không hợp lệ.");

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180).WithMessage("Tọa độ Kinh độ (Longitude) không hợp lệ.");
        }
    }
}
