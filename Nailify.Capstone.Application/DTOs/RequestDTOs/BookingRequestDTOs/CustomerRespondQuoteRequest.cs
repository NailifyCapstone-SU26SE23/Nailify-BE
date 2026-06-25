using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs
{
    public class CustomerRespondQuoteRequest
    {
        public bool IsAccepted { get; set; }
        public string? RejectReason { get; set; }
    }
}
