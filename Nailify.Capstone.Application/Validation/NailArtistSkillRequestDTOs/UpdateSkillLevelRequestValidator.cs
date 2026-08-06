using FluentValidation;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailArtistSkillRequestDTOs;

namespace Nailify.Capstone.Application.Validation.NailArtistSkillRequestDTOs
{
    public class UpdateSkillLevelRequestValidator : AbstractValidator<UpdateSkillLevelRequest>
    {
        public UpdateSkillLevelRequestValidator()
        {
            RuleFor(x => x.RequiredLevel)
                .InclusiveBetween(1, 5)
                .WithMessage("Cấp độ kỹ năng (RequiredLevel) phải từ 1 đến 5 sao.");
        }
    }
}
