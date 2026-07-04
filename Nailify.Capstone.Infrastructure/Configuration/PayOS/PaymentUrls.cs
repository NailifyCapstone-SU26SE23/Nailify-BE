using Nailify.Capstone.Application.Interfaces.ConfigurationInterfaces;

namespace Nailify.Capstone.Infrastructure.Configuration.PayOS
{
    public class PaymentUrls : IPaymentUrls
    {
        public string ReturnUrl { get; set; } = string.Empty;
        public string CancelUrl { get; set; } = string.Empty;
    }
}
