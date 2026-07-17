namespace Nailify.Capstone.Application.Interfaces.ConfigurationInterfaces
{
    public interface INemotronConfiguration
    {
        string LlmProvider { get; set; }
        string OpenRouterApiKey { get; set; }
        string OpenRouterModel { get; set; }
        string OpenRouterBaseUrl { get; set; }
    }
}
