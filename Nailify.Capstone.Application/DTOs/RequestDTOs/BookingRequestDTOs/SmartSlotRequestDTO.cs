using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs
{
    public class SmartSlotRequestDTO
    {
        public Guid SalonId { get; set; }
        public DateTime Date { get; set; }

        public List<BookingItemRequestDTO> BookingItems { get; set; } = new();
    }
}
