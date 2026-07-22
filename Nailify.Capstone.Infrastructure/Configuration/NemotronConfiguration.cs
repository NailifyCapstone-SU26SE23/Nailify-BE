using Nailify.Capstone.Application.Interfaces.ConfigurationInterfaces;

namespace Nailify.Capstone.Infrastructure.Configuration
{
    public class NemotronConfiguration : INemotronConfiguration
    {
        public string LlmProvider { get; set; } = "openrouter";
        public string OpenRouterApiKey { get; set; } = string.Empty;
        public string OpenRouterModel { get; set; } = "nvidia/nemotron-3-nano-30b-a3b:free";
        public string OpenRouterBaseUrl { get; set; } = "https://openrouter.ai/api/v1/chat/completions";
    }
}
