using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Services
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private static readonly string _baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string _templateFolder = Path.Combine(_baseDirectory, "EmailTemplate");

        private string ReadEmailTemplateContent(string templateName)
        {
            var filePath = Path.Combine(_templateFolder, templateName + ".html");
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Không tìm thấy email template tại: {filePath}");
            }
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var sr = new StreamReader(fs);
            var content = sr.ReadToEnd();
            sr.Close();
            return content;
        }
        public string GenerateBookingReminderEmail(string username, string salonName, string startTime, string bookingDate)
        {
            var template = ReadEmailTemplateContent("booking-reminder");
            return template.Replace("[username]", username)
                           .Replace("[salonName]", salonName)
                           .Replace("[startTime]", startTime)
                           .Replace("[bookingDate]", bookingDate);
        }
        public string GenerateWaitlistConfirmationEmail(string username, string startTime, string requestedDate, string confirmUrl)
        {
            var template = ReadEmailTemplateContent("waitlist-confirmation");
            return template.Replace("[username]", username)
                           .Replace("[startTime]", startTime)
                           .Replace("[requestedDate]", requestedDate)
                           .Replace("[confirmUrl]", confirmUrl);
        }
        public string GenerateForgotPasswordEmail(string username, string resetCode)
        {
            var template = ReadEmailTemplateContent("forgot-password");
            return template.Replace("[username]", username)
                           .Replace("[resetCode]", resetCode);
        }
    }
}
