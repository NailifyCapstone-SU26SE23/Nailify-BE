using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Domain.Entities
{
    public class Customer
    {
        public Guid UserId { get; set; } //  PK và FK sang bảng User
        public int LoyaltyPoint { get; set; } = 0;
        public string? SkinTone { get; set; } = string.Empty;
        public string? Occupation { get; set; } = string.Empty;
        public string? NailCondition { get; set; } = string.Empty;
        public string? PersonaId { get; set; } = string.Empty;

        public virtual User User { get; set; } = null!;
    }
}
