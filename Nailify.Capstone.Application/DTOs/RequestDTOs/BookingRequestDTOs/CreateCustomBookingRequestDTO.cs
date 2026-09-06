using System;
using System.Collections.Generic;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs
{
    public class CreateCustomBookingRequestDTO
    {
        public Guid SalonId { get; set; }
        public int CustomerNailId { get; set; }
        public DateTime BookingDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public List<CustomServiceItemRequestDTO>? AdditionalServices { get; set; }
    }
}
