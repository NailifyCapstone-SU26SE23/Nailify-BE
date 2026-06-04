using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.UserRequestDTOs
{
    public class CustomerPreferencesUpdateRequest
    {
        public string SkinTone { get; set; } = string.Empty;
        public string Occupation { get; set; } = string.Empty;
        public string NailCondition { get; set; } = string.Empty;
        public string PersonaId { get; set; } = string.Empty;
    }
}
