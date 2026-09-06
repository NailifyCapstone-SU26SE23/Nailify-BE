using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs
{
    public class ManagerApproveQuoteRequestDTO
    {
        public decimal? FinalPrice { get; set; }
        public int? FinalDuration { get; set; }
    }
}
