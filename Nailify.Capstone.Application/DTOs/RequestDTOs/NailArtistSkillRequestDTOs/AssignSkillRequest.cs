using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.NailArtistSkillRequestDTOs
{
    // tạo thủ công
    // Gán skill
    public class AssignSkillRequest
    {
        public Guid SkillTypeId { get; set; }
        public int Level { get; set; }
    }
}
