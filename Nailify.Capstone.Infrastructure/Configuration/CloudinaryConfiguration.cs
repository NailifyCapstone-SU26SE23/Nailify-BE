using Nailify.Capstone.Application.Interfaces.ConfigurationInterfaces;

namespace Nailify.Capstone.Infrastructure.Configuration
{
    public class CloudinaryConfiguration : ICloudinaryConfiguration
    {
        public string CloudName { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string ApiSecret { get; set; } = string.Empty;
    }
}
