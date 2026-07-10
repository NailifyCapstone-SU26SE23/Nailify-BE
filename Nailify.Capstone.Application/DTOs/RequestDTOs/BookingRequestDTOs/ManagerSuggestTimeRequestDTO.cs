using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs
{
    public class ManagerSuggestTimeRequestDTO
    {
        public DateTime SuggestedDate { get; set; }
        public TimeSpan SuggestedTime { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
