using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs;
using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.PaymentDTOs
{
    public class PayOSPaymentContextRequest
    {
        /// <summary>
        ///  Ngữ cảnh thanh toán
        /// </summary>
        public PaymentContextType ContextType { get; set; }
        public Guid? BookingId { get; set; }
        public Guid? WalletId { get; set; }
        public Guid? CustomerId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = null!;
        public string ItemName { get; set; } = null!;
        public string? CustomReturnUrl { get; set; }
        public string? CustomCancelUrl { get; set; }
        public CreateBookingRequestDTO? BookingRequestData { get; set; }
    }
}
