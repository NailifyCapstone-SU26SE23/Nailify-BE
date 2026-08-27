using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailArtistRequestDTOs;

namespace Nailify.Capstone.Application.Validation.NailArtistRequestDTOs
{
    public class NailArtistUpdateRequestValidator : AbstractValidator<NailArtistUpdateRequest>
    {
        public NailArtistUpdateRequestValidator()
        {
            RuleFor(x => x.SalonId)
                .NotEmpty().WithMessage("Vui lòng chọn chi nhánh Salon cho thợ móng.");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Trạng thái thợ móng không được để trống.")
                .Must(status => status == "Active" || status == "Inactive" || status == "OnBreak")
                .WithMessage("Trạng thái thợ móng phải là 'Active', 'Inactive' hoặc 'OnBreak'.");
        }
    }
}
