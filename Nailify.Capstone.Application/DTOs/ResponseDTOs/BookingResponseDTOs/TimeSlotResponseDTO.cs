using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingResponseDTOs
{
    public class TimeSlotResponseDTO
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsHeld { get; set; }
    }
}
