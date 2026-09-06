using Nailify.Capstone.Application.Interfaces.ConfigurationInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Infrastructure.Configuration
{
    public class SmtpEmailConfiguration : IEmailConfiguration
    {
        public string DisplayName { get; set; } = string.Empty;
        public string From { get; set; } = string.Empty;
        public string SMTPServer { get; set; } = string.Empty;
        public int Port { get; set; }
        public bool UseSsL { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
