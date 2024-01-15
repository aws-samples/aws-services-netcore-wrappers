using Amazon.ElastiCache.Wrapper.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amazon.ElastiCache.Wrapper.Implementations
{
    public class RedisCacheRepository : IRedisCacheRepository
    {
        private readonly IDistributedCache _cache;

        public RedisCacheRepository(IDistributedCache distributedCache) {
            _cache = distributedCache;
        }
        public async Task<string> GetStringAsync(string key)
        {
            try
            {
                var cache = await _cache.GetStringAsync(key);
                return cache != null ? cache : string.Empty;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task SetStringAsync(string key, string value)
        {
            try
            {
                await _cache.SetStringAsync(key, value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                await _cache.RemoveAsync(key);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
