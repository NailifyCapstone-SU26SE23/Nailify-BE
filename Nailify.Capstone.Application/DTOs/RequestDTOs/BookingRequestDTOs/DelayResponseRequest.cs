using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs
{
    public class DelayResponseRequest
    {
        public DelayCustomerDecision CustomerDecision { get; set; }
        public DateTime? NewDate { get; set; }
        public TimeSpan? NewTime { get; set; }
    }
}
