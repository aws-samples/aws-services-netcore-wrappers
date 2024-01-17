using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amazon.ElastiCache.Wrapper.Configuration
{
    public class ElasticCacheOptions
    {
        public string? ServerEndpoint { get; set; }

        public string? InstanceName { get; set; }
    }
}
