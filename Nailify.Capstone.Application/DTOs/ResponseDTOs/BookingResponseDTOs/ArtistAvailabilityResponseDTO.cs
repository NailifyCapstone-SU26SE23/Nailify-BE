using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingResponseDTOs
{
    public class ArtistAvailabilityResponseDTO
    {
        public Guid NailArtistId { get; set; }
        public string ArtistName { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;

        public string AvailabilityStatus { get; set; } = string.Empty;
        public TimeSpan ShiftStart { get; set; }
        public TimeSpan ShiftEnd { get; set; }
        public List<BusyTimeSlotResponseDto> BusySlots { get; set; } = new();
    }
}
