using Nailify.Capstone.Application.Interfaces.ConfigurationInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Infrastructure.Configuration
{
    public class RedisConfiguration : IRedisConfiguration
    {
        public string ConnectionString { get; set; } = "localhost:6379";
        public string InstanceName { get; set; } = "Nailify_";
        public bool UseMemoryCache { get; set; } = false;
    }
}
