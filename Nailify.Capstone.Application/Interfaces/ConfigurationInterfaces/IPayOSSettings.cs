namespace Nailify.Capstone.Application.Interfaces.ConfigurationInterfaces
{
    public interface IPayOSSettings
    {
        string ClientId { get; }
        string ApiKey { get; }
        string ChecksumKey { get; }
        string PayoutClientId { get; }
        string PayoutApiKey { get; }
        string PayoutChecksumKey { get; }
    }
}
