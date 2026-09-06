using Microsoft.Extensions.Logging;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Nailify.Capstone.Application.DTOs.RequestDTOs.MailRequestDTO;
using Nailify.Capstone.Application.Interfaces.ConfigurationInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using System;
using System.Threading.Tasks;

namespace Nailify.Capstone.Infrastructure.Service
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IEmailConfiguration _settings;
        private readonly ILogger<SmtpEmailService> _logger;

        public SmtpEmailService(IEmailConfiguration settings, ILogger<SmtpEmailService> logger)
        {
            _settings = settings;
            _logger = logger;
        }

        public async Task SendEmailAsync(MailRequest request)
        {
            try
            {
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress(_settings.DisplayName, _settings.From));
                emailMessage.To.Add(MailboxAddress.Parse(request.ToAddress));
                emailMessage.Subject = request.Subject;
                var bodyBuilder = new BodyBuilder { HtmlBody = request.Body };
                emailMessage.Body = bodyBuilder.ToMessageBody();

                using var smtpClient = new SmtpClient();
                smtpClient.Timeout = 10000; // 10 seconds timeout limit

                var socketOption = _settings.UseSsL ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls;
                await smtpClient.ConnectAsync(_settings.SMTPServer, _settings.Port, socketOption);
                await smtpClient.AuthenticateAsync(_settings.UserName, _settings.Password);
                await smtpClient.SendAsync(emailMessage);
                await smtpClient.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {ToAddress} via SMTP server {Server}:{Port}", request.ToAddress, _settings.SMTPServer, _settings.Port);
            }
        }
    }
}

