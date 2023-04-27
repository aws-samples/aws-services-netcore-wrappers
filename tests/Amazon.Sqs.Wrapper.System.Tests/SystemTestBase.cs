using Amazon.SQS;
using Amazon.Sqs.Wrapper.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Amazon.Sqs.Wrapper.System.Tests
{
    public abstract class SystemTestBase
    {
        public IAmazonSQS SqsClient { get; set; }
        public IOptions<SqsConfig> SqsConfig { get; set; }

        public SystemTestBase()
        {
            SqsClient = new AmazonSQSClient();
            SqsConfig = GetSqsConfig();

        }
        private IOptions<SqsConfig> GetSqsConfig()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var sqlConfig = Options.Create(configuration.GetSection("SqsConfig").Get<SqsConfig>());
            var uniqueId = Guid.NewGuid();
            sqlConfig.Value.QueueName = $"{sqlConfig.Value.QueueName}{uniqueId}";
            sqlConfig.Value.QueueUrl = $"{sqlConfig.Value.QueueUrl}{uniqueId}";
            return sqlConfig;
        }
    }
}
