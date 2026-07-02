namespace Nailify.Capstone.Infrastructure.Configuration
{
    public class PaymentWebhookDto
    {
        public string Code { get; set; } = string.Empty;
        public string Desc { get; set; } = string.Empty;
        public bool Success { get; set; }
        public PaymentWebhookData? Data { get; set; }
        public string? Signature { get; set; }

        // For signature verification
        public long OrderCode { get; set; }
        public decimal Amount { get; set; }
    }
}
