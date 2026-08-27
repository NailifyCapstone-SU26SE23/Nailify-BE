using Microsoft.Extensions.Logging;
using Nailify.Capstone.Application.DTOs.RequestDTOs.MailRequestDTO;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Infrastructure.Configuration;
using System.Net;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Nailify.Capstone.Infrastructure.Service
{
    public class SendGridEmailService : IEmailService
    {
        private readonly SendGridEmailConfiguration _settings;
        private readonly ILogger<SendGridEmailService> _logger;

        public SendGridEmailService(
            SendGridEmailConfiguration settings,
            ILogger<SendGridEmailService> logger)
        {
            _settings = settings;
            _logger = logger;
        }

        public async Task SendEmailAsync(MailRequest request)
        {
            if (string.IsNullOrWhiteSpace(_settings.ApiKey))
            {
                _logger.LogError("SendGrid API key is not configured.");
                throw new InvalidOperationException("SendGrid API key is not configured.");
            }

            try
            {
                var client = new SendGridClient(_settings.ApiKey);
                var from = new EmailAddress(_settings.From, _settings.DisplayName);
                var to = new EmailAddress(request.ToAddress);
                var message = MailHelper.CreateSingleEmail(from, to, request.Subject, plainTextContent: null, htmlContent: request.Body);

                var response = await client.SendEmailAsync(message);

                if (response.StatusCode != HttpStatusCode.Accepted)
                {
                    var error = await response.Body.ReadAsStringAsync();
                    _logger.LogError(
                        "SendGrid failed to send email to {ToAddress}. StatusCode: {StatusCode}. Response: {Response}",
                        request.ToAddress,
                        response.StatusCode,
                        error);

                    throw new InvalidOperationException($"SendGrid error: {response.StatusCode}");
                }

                _logger.LogInformation("Email sent to {ToAddress} via SendGrid.", request.ToAddress);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {ToAddress} via SendGrid.", request.ToAddress);
                throw;
            }
        }
    }
}
