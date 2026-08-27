using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs
{
    public class ProcedureQuoteItemDTO
    {
        public Guid? ProcedureId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int EstimatedMinutes { get; set; }
        public decimal Price { get; set; }
        public int StepOrder { get; set; }
        public bool IsCustomStep { get; set; }
        public string? Note { get; set; }
    }

    public class ArtistQuoteRequestDTO
    {
        public decimal? QuotedPrice { get; set; }
        public int? QuotedDuration { get; set; }
        public string? ArtistNotes { get; set; }
        public List<ProcedureQuoteItemDTO>? Procedures { get; set; }
    }
}
