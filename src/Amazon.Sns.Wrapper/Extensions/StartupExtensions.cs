using Amazon.SimpleNotificationService;
using Amazon.Sns.Wrapper.Implementations;
using Amazon.Sns.Wrapper.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amazon.Sns.Wrapper.Extensions
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddSnsClient(this IServiceCollection services,
                                                        IConfiguration config,
                                                        string snsConfigSectionName)
        {
            services.AddAWSService<IAmazonSimpleNotificationService>(config.GetAWSOptions(snsConfigSectionName));
            return services.AddSingleton< INotificationService,NotificationService>();
        }
    }
}
