using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.ConfigurationInterfaces
{
    public interface ICloudinaryConfiguration
    {
        string CloudName { get; set; }
        string ApiKey { get; set; }
        string ApiSecret { get; set; }
    }
}
