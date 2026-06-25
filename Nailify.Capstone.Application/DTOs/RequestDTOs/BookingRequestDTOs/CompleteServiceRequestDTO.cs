using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs
{
    public class CompleteServiceRequestDTO
    {
        public Guid BookingId { get; set; }
        public List<string> CompleteImagesUrl { get; set; } = new();
    }
}
