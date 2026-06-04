using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs
{
    public class CustomerProfileDto
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        // Thông tin đặc thù mở rộng từ bảng Customer
        public int LoyaltyPoint { get; set; }
        public string SkinTone { get; set; } = string.Empty;
        public string Occupation { get; set; } = string.Empty;
        public string NailCondition { get; set; } = string.Empty;
        public string PersonaId { get; set; } = string.Empty;
    }
}
