using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.UserRequestDTOs
{
    public class CustomerPreferencesUpdateRequest : IMapFrom<Customer>
    {
        public string SkinTone { get; set; } = string.Empty;
        public string Occupation { get; set; } = string.Empty;
        public string NailCondition { get; set; } = string.Empty;
        public string PersonaId { get; set; } = string.Empty;
    }
}
