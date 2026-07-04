namespace Nailify.Capstone.Infrastructure.Configuration.PayOS
{
    public class PayoutResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string TransactionId { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
    }
}
