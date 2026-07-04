using System.ComponentModel.DataAnnotations;

namespace Nailify.Capstone.Domain.Entities
{
    public enum TransactionStatus
    {
        [Display(Name = "Chờ thanh toán")]
        Pending,

        [Display(Name = "Đã thanh toán")]
        Paid,

        [Display(Name = "Quá hạn")]
        Overdue,

        [Display(Name = "Đã hủy")]
        Cancelled,

        [Display(Name = "Đã hoàn tiền")]
        Refunded
    }

    public class Transaction
    {
        public int TransactionId { get; set; }
        public Guid BookingId { get; set; }
        public Booking Booking { get; set; } = null!;

        public string OrderCode { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? Reference { get; set; } 
        public string? PaymentLinkId { get; set; } 
        public string CheckoutUrl { get; set; } = string.Empty;
        public string QrCode { get; set; } = string.Empty;
        public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
        public string Policy { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime ExpiresAt { get; set; } 
        public string WebhookPayload { get; set; } = string.Empty;
    }
}
