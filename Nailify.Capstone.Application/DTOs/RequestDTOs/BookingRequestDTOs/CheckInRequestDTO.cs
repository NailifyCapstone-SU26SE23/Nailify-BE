using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs
{
    public class CheckInRequestDTO
    {
        public Guid BookingId { get; set; }
        public string CheckInImageUrl { get; set; } = string.Empty;
    }
}
