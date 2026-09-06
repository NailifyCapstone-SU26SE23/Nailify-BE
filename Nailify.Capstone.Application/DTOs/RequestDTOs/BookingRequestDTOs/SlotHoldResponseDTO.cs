using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs
{
    public class SlotHoldResponseDTO
    {
        public string HoldToken { get; set; } // unique token để confirm
        public DateTime ExpiresAt { get; set; }   // thời điểm hết hạn giữ chỗ
        public int RemainingSeconds { get; set; }  // số giây còn lại
        public bool IsHeld { get; set; }
    }
}
