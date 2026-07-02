using Nailify.Capstone.Application.Interfaces.ConfigurationInterfaces;

namespace Nailify.Capstone.Infrastructure.Configuration
{
    public class PayOSSettings : IPayOSSettings
    {
        public string ClientId { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string ChecksumKey { get; set; } = string.Empty;
    }
}
