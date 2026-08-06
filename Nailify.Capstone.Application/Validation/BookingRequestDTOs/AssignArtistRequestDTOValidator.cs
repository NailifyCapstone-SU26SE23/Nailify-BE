using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs;

namespace Nailify.Capstone.Application.Validation.BookingRequestDTOs
{
    public class AssignArtistRequestDTOValidator : AbstractValidator<AssignArtistRequestDTO>
    {
        public AssignArtistRequestDTOValidator()
        {
            RuleFor(x => x.StaffArtistId)
                .NotEmpty().WithMessage("Vui lòng chọn thợ móng (StaffArtistId) để điều phối lịch hẹn.");
        }
    }
}
