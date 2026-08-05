namespace Nailify.Capstone.Infrastructure.Configuration
{
    public class SendGridEmailConfiguration
    {
        public string ApiKey { get; set; } = string.Empty;
        public string From { get; set; } = "nailify.capstone@gmail.com";
        public string DisplayName { get; set; } = "Nailify Center";
    }
}
