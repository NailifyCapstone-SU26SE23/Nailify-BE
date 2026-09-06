namespace Nailify.Capstone.Application.Interfaces.ConfigurationInterfaces
{
    public interface IPaymentUrls
    {
        string ReturnUrl { get; }
        string CancelUrl { get; }
    }
}
