using Nailify.Capstone.Application.Interfaces.ConfigurationInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Infrastructure.Configuration
{
    public class SlotHoldConfiguration : ISlotHoldConfiguration
    {
        public int HoldDurationSeconds { get; set; } = 300;
        public string KeyPrefix { get; set; } = "slot_hold";
    }
}
