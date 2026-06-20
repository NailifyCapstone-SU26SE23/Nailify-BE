using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.ConfigurationInterfaces
{
    public interface ISlotHoldConfiguration
    {
        int HoldDurationSeconds { get; set; }
        string KeyPrefix { get; set; }
    }
}
