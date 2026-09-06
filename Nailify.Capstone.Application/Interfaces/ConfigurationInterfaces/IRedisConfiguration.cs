using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.ConfigurationInterfaces
{
    public interface IRedisConfiguration
    {
        string ConnectionString { get; set; }
        string InstanceName { get; set; }
        bool UseMemoryCache { get; set; }
    }
}
