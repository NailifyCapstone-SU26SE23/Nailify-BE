using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.LoyaltyTransactionRequestDTOs
{
    public class RefundPointsRequest
    {
        public Guid CustomerId { get; set; }
        public Guid? BookingId { get; set; }
        public int PointsToRefund { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
