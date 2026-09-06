using System;
using System.Collections.Generic;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs
{
    public class UpdateBookingRequestDTO
    {
        public DateTime BookingDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public Guid? NailArtistId { get; set; }
        public List<int>? SelectedPromotionIds { get; set; }
        public List<BookingItemRequestDTO> BookingItems { get; set; } = new();
    }
}
