using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amazon.ElastiCache.Wrapper.Interfaces
{
    public interface IRedisCacheRepository
    {
        public Task<string> GetStringAsync(string key);
        public Task SetStringAsync(string key, string value);
        public Task RemoveAsync(string key);
    }
}
