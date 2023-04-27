using Amazon.SQS;
using Amazon.Sqs.Wrapper.Configuration;
using Amazon.Sqs.Wrapper.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Amazon.Sqs.Wrapper.Extensions
{
    /// <summary>
    /// Startup extention to inject SQS Dependencies
    /// </summary>
    /// <param name="sqsConfigSectionName">sqsConfigSectionName</param>
    /// sqsConfigSectionName defines the section in config that maps to json
    // structure as per SqsConfig class in Configuration folder

    public static class SqsExtension
    {
        public static IServiceCollection RegisterSqsClient(this IServiceCollection services, 
                                                        IConfiguration config, 
                                                        string sqsConfigSectionName)
        {
            services.Configure<SqsConfig>(config.GetSection(sqsConfigSectionName));
            services.AddScoped<IMessageQueue, SqsMessageQueue>();
            return services.AddAWSService<IAmazonSQS>(config.GetAWSOptions(sqsConfigSectionName));
        }
    }
}
