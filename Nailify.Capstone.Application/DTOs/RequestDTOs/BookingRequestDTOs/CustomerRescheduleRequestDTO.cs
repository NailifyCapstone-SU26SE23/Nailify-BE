using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs
{
    public class CustomerRescheduleRequestDTO
    {
        public DateTime NewDate { get; set; }
        public TimeSpan NewTime { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
