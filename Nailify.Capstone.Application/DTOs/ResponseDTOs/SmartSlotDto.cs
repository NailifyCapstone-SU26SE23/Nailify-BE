using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs
{
    public class SmartSlotDto
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public Guid AssignedArtistId { get; set; }
        public string ArtistName { get; set; } = string.Empty;
        public double PriorityScore { get; set; }
        public string RecommendationReason { get; set; } = string.Empty;
    }
}
