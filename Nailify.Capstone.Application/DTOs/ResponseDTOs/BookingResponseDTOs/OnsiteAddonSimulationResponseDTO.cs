using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingResponseDTOs
{
    // Thợ có rảnh ko? được phép thêm dịch vụ này không?
    public class OnsiteAddonSimulationResponseDTO
    {
        public bool HasConflict { get; set; }
        public bool CanMultiArtistSplit { get; set; }
        public Guid? PrimaryArtistId { get; set; }
        public string? PrimaryArtistName { get; set; }
        public Guid? SuggestedSecondaryArtistId { get; set; }
        public string? SuggestedSecondaryArtistName { get; set; }
        public List<string> AddonNames { get; set; } = new List<string>();
        public int TotalAddonDurationMinutes { get; set; }
        public decimal TotalAddonPrice { get; set; }
        public string WarningMessage { get; set; } = string.Empty;
        public string RecommendationMessage { get; set; } = string.Empty;
        public TimeSpan? SuggestedAlternativeTime { get; set; }
        public int NewTotalDurationMinutes { get; set; }
        public decimal NewTotalPrice { get; set; }
    }
}
