using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.WaitlistRequestDTOs
{
    public class ConfirmWaitlistRequestDTO
    {
        public List<BookingItemRequestDTO> BookingItems { get; set; } = new();
        public List<int>? SelectedPromotionIds { get; set; }
        public bool UseWalletBalance { get; set; } = false; // Cho phép trừ cọc
    }
}
