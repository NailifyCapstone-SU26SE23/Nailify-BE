using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.ServiceRequestDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Validation.ServiceRequestDTOs
{
    public class ServiceUpdateRequestValidator : AbstractValidator<ServiceUpdateRequestDTO>
    {
        public ServiceUpdateRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tên dịch vụ không được để trống.")
                .MaximumLength(150).WithMessage("Tên dịch vụ không vượt quá 150 ký tự.");
            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Giá dịch vụ không được âm.");
            RuleFor(x => x.Duration)
                .GreaterThan(0).WithMessage("Thời lượng dịch vụ phải lớn hơn 0 phút.");
            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Trạng thái dịch vụ không được để trống.")
                .Must(status => status == "Active" || status == "InActive")
                .WithMessage("Trạng thái phải là 'Active' hoặc 'InActive'.");
        }
    }
}
