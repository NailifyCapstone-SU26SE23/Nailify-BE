using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.NailArtistBreakRequestDTOs
{
    public class ApproveRejectBreakRequest
    {
        public ArtistBreakStatus Status { get; set; }
        public string? RejectReason { get; set; }
    }
}
