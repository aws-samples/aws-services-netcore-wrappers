using Amazon.Extensions.NETCore.Setup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Amazon.ElastiCache.Wrapper.Interfaces;
using Amazon.ElastiCache.Wrapper.Implementations;
using Amazon.ElastiCache.Wrapper.Configuration;
using System;

namespace Amazon.ElastiCache.Wrapper.Extensions
{
    public static class StartupExtentions
    {
        private const string ELASTICCACHE_SECTION = "ElastiCache";

        public static IServiceCollection RegisterElasticCacheServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            // Get the AWS profile information from configuration providers
            AWSOptions awsOptions = configuration.GetAWSOptions();

            // Configure AWS service clients to use these credentials
            services.AddDefaultAWSOptions(awsOptions);

            var elasticCacheConfigSection = configuration.GetSection(ELASTICCACHE_SECTION);
            services.Configure<ElasticCacheOptions>(elasticCacheConfigSection);

            var elasticCacheConfig = new ElasticCacheOptions();
            elasticCacheConfigSection.Bind(elasticCacheConfig);

            //Service registration for cache
            Console.WriteLine("ServerEndpoint : " + elasticCacheConfig.ServerEndpoint);

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = elasticCacheConfig.ServerEndpoint;
                options.InstanceName = elasticCacheConfig.InstanceName;
            });

            services.AddSingleton<IRedisCacheRepository, RedisCacheRepository>();

            return services;
        }
    }
}
