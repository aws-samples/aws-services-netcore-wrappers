using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Amazon.SecretManager.Wrapper
{
     /// <summary>
    /// Provides a simple implementation to fetch secrets from AWS Secrets Manager and return it as Dictionary of <K,V> pair
    /// </summary>
    public class SecretManagerProvider : ConfigurationProvider, IConfigurationSource
    {
        internal readonly string secretName;
        internal readonly string region;

        public SecretManagerProvider(string secretName)
        {
            this.secretName = secretName;
        }

        public SecretManagerProvider(string secretName, string region)
        {
            this.secretName = secretName;
            this.region = region;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return this;
        }

        public override void Load()
        {
            try
            {
                Data = LoadConfiguration();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private IDictionary<string, string> LoadConfiguration()
        {
            string encryptedText = "";

            IAmazonSecretsManager client = !string.IsNullOrWhiteSpace(region) ? new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(region)) : new AmazonSecretsManagerClient();
            GetSecretValueRequest request = new GetSecretValueRequest
            {
                SecretId = secretName
            };

            var response = client.GetSecretValueAsync(request).GetAwaiter().GetResult();

            if (response != null)
            {
                if (response.SecretString != null)
                {
                    encryptedText = response.SecretString;
                }
                else
                {
                    var memoryStream = response.SecretBinary;
                    using (StreamReader reader = new StreamReader(memoryStream))
                    {
                        string secretString = reader.ReadToEnd();
                        byte[] secretBinary = Convert.FromBase64String(secretString);
                        encryptedText = Encoding.UTF8.GetString(secretBinary);
                    }
                }
            }

            return JsonConvert.DeserializeObject<IDictionary<string, string>>(encryptedText);
        }
    }
}
