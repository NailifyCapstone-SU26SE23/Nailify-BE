using Nailify.Capstone.Application.Interfaces.ConfigurationInterfaces;

namespace Nailify.Capstone.Infrastructure.Configuration
{
    public class GoogleConfiguration : IGoogleConfiguration
    {
        public string ClientId { get; set; } = string.Empty;
    }
}
