using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs
{
    public class CustomServiceItemRequestDTO
    {
        public Guid ServiceId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
