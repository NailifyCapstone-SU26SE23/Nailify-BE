using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs
{
    // Giả lập để chạy lấy thời gian
    public class SimulateOnsiteAddonRequestDTO
    {
        public Guid BookingId { get; set; }
        public List<AddonItemRequestDTO> AddonItems { get; set; } = new List<AddonItemRequestDTO>();
    }
    public class ConfirmOnsiteAddonRequestDTO : SimulateOnsiteAddonRequestDTO
    {
        public Guid? AssignedArtistId { get; set; } 
    }
    public class AddonItemRequestDTO
    {
        public Guid? ServiceId { get; set; }
        public int? NailVariantId { get; set; }
        public int? ShapeMethodConfigId { get; set; }
        public int? CustomerNailId { get; set; }
        public Guid? CustomerNailRequestId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
