using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs
{
    public class HoldSlotRequestDTO
    {
        public Guid SalonId { get; set; }
        public Guid NailArtistId { get; set; }
        public DateTime BookingDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public List<BookingItemRequestDTO>? BookingItems { get; set; } = new();
    }
}
