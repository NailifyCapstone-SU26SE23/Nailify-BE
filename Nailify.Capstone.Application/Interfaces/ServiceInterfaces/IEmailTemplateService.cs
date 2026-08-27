using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface IEmailTemplateService
    {
        string GenerateBookingReminderEmail(string username, string salonName, string startTime, string bookingDate);
        string GenerateWaitlistConfirmationEmail(string username, string startTime, string requestedDate, string confirmUrl);
        string GenerateForgotPasswordEmail(string username, string resetCode);
    }
}
