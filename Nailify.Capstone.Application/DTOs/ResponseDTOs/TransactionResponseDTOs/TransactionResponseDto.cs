using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.TransactionResponseDTOs
{
    public class TransactionResponseDto
    {
        public int TransactionId { get; set; }
        public Guid BookingId { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? Reference { get; set; }
        public string? PaymentLinkId { get; set; }
        public string? Reason { get; set; }
        public string Policy { get; set; } = string.Empty;
        public string CheckoutUrl { get; set; } = string.Empty;
        public string QrCode { get; set; } = string.Empty;
        public TransactionStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public Guid SalonId { get; set; }
        public string SalonName { get; set; } = string.Empty;
    }
}
