using System;
using System.Collections.Generic;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs
{
    public class GetRandomArtistRequestDTO
    {
        public Guid SalonId { get; set; }
        public DateTime BookingDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public List<BookingItemRequestDTO> BookingItems { get; set; } = new();
    }
}
