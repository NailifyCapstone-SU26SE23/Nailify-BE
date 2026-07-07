using System;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingResponseDTOs
{
    public class IdleArtistResponseDTO
    {
        public Guid NailArtistId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsFree { get; set; }
        public bool IsQualified { get; set; }
    }
}
