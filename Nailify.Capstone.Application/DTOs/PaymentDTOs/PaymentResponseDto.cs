namespace Nailify.Capstone.Application.DTOs.PaymentDTOs
{
    public class PaymentResponseDto
    {
        public string PaymentUrl { get; set; } = string.Empty;
        public string ReturnUrl { get; set; } = string.Empty;
        public string CancelUrl { get; set; } = string.Empty;
        public string QrCode { get; set; } = string.Empty;
        public long OrderCode { get; set; }
        public string Status { get; set; } = string.Empty;
        public int TransactionId { get; set; }
        public Guid? BookingId { get; set; }
        public decimal Amount { get; set; }
    }
}
