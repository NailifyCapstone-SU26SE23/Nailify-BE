using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.ConfigurationInterfaces
{
    public interface IEmailConfiguration
    {
        string DisplayName { get; set; }
        string From { get; set; }
        string SMTPServer { get; set; }
        int Port { get; set; }
        bool UseSsL { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
    }
}
