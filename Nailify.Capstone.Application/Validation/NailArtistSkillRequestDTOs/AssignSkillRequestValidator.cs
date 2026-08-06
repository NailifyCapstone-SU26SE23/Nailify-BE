using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailArtistSkillRequestDTOs;

namespace Nailify.Capstone.Application.Validation.NailArtistSkillRequestDTOs
{
    public class AssignSkillRequestValidator : AbstractValidator<AssignSkillRequest>
    {
        public AssignSkillRequestValidator()
        {
            RuleFor(x => x.SkillTypeId)
                .NotEmpty().WithMessage("Vui lòng chọn loại kỹ năng (SkillType).");

            RuleFor(x => x.Level)
                .InclusiveBetween(1, 5)
                .WithMessage("Cấp độ kỹ năng (Level) phải từ 1 đến 5 sao.");
        }
    }
}
