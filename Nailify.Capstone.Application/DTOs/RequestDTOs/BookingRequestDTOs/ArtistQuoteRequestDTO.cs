using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs
{
    public class ArtistQuoteRequestDTO
    {
        public decimal QuotedPrice { get; set; }
        public int QuotedDuration { get; set; }
    }
}
