using Amazon.Extensions.NETCore.Setup;
using Amazon.SecurityToken;
using Amazon.SimpleEmail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Amazon.Ses.Wrapper
{
    public static class SesStartupExtension
    {
        public static IServiceCollection RegisterSesServices(this IServiceCollection services,
            IConfiguration configuration,
            string sesConfigSectionName= "SESOptions")
        {
            // Get the AWS profile information from configuration providers
            AWSOptions awsOptions = configuration.GetAWSOptions();

            // Configure AWS service clients to use these credentials
            services.AddDefaultAWSOptions(awsOptions);

            var sesConfigSection = configuration.GetSection(sesConfigSectionName);
            services.Configure<SESOptions>(sesConfigSection);

            var sesConfig = new SESOptions();
            sesConfigSection.Bind(sesConfig);

            //AssumeRoleFlag is required to inject IAmazonSecurityTokenService else default role will be assumed
            if (sesConfig.AssumeRoleFlag)
            {
                services.AddSingleton<IAmazonSecurityTokenService, AmazonSecurityTokenServiceClient>();
            }
            services.AddSingleton<IAmazonSimpleEmailService, AmazonSimpleEmailServiceClient>();
            services.AddSingleton<ISESEmailService, SESEmailService>();
            return services;
        }
    }
}
