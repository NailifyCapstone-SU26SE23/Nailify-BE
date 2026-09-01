using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.SalonResponseDTOs
{
    public class SalonAvailabilityResponseDTO
    {
        public Guid SalonId { get; set; }
        public TimeSpan SalonOpenTime { get; set; }
        public TimeSpan SalonCloseTime { get; set; }
        public List<SalonTimeSlotResponseDTO> TimeSlots { get; set; } = new();
    }
}
