using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs
{
    public class CheckOutRequestDTO
    {
        public Guid BookingId { get; set; }
        public List<string> CheckOutImagesUrl { get; set; } = new();
    }
}
