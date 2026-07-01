using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MimeKit;
using Nailify.Capstone.Application.DTOs.RequestDTOs.MailRequestDTO;
using Nailify.Capstone.Application.Interfaces.ConfigurationInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Infrastructure.Service
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IEmailConfiguration _settings;

        public SmtpEmailService(IEmailConfiguration settings)
        {
            _settings = settings;
        }

        public async Task SendEmailAsync(MailRequest request)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_settings.DisplayName, _settings.From));
            emailMessage.To.Add(MailboxAddress.Parse(request.ToAddress));
            emailMessage.Subject = request.Subject;
            var bodyBuilder = new BodyBuilder { HtmlBody = request.Body };
            emailMessage.Body = bodyBuilder.ToMessageBody();
            using var smtpClient = new SmtpClient();
            try
            {
                await smtpClient.ConnectAsync(_settings.SMTPServer, _settings.Port, _settings.UseSsL);
                await smtpClient.AuthenticateAsync(_settings.UserName, _settings.Password);
                await smtpClient.SendAsync(emailMessage);
            }
            finally
            {
                await smtpClient.DisconnectAsync(true);
                smtpClient.Dispose();
            }
        }
    }
}
