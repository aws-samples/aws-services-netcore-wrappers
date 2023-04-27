using Microsoft.Extensions.Configuration;

namespace Amazon.SecretManager.Wrapper
{
     /// <summary>
    /// Provides extension methods to AddSecrets from AWS Secrets Manager
    /// </summary>
    public static class SecretManagerProviderExtension
    {
        public static IConfigurationBuilder AddSecrets(this IConfigurationBuilder builder, string secretName)
        {
            return builder.Add(new SecretManagerProvider(secretName));
        }

        public static IConfigurationBuilder AddSecrets(this IConfigurationBuilder builder, string secretName, string region)
        {
            return builder.Add(new SecretManagerProvider(secretName, region));
        }
    }
}
