using Nailify.Capstone.Application.DTOs.RequestDTOs.NailArtistRequestDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.NailArtistResponseDTOs
{
    public class NailArtistTransferResultDTO
    {
        public NailArtistTransferResponseDTO Transfer { get; set; }
        public int TotalAffectedBookings { get; set; }
        public int AutoReassignedCount { get; set; }
        public int RescheduledSuggestCount { get; set; }
        public int CancelledCount { get; set; }
        public List<EmergencyBookingHandlingDetailDTO> ProcessingDetails { get; set; } = new();
    }
}
