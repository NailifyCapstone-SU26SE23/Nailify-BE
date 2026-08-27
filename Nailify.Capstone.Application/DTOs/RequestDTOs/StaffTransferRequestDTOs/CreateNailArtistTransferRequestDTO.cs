using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.StaffTransferRequestDTOs
{
    public class CreateNailArtistTransferRequestDTO
    {
        public Guid NailArtistId { get; set; }
        public Guid ToSalonId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Reason { get; set; }
    }
}
