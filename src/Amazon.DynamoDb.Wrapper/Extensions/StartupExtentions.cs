using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Extensions.NETCore.Setup;
using Amazon.DynamoDb.Wrapper.Implementations;
using Amazon.DynamoDb.Wrapper.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Amazon.DynamoDb.Wrapper.Extensions
{
    public static class StartupExtentions
    {
        public static string tableNamePrefix = string.Empty;

        private const string DYNAMODB_SECTION = "DynamoDb";
        private const string DYNAMODB_SECTION_LOCALMODE = "LocalMode";
        private const string DYNAMODB_SECTION_LOCALSERVICEURL = "LocalServiceUrl";

        public static IServiceCollection RegisterDynamoDBServices(this IServiceCollection services,
            IConfiguration configuration,
            string tableNamePrefixParam = "")
        {
            // Get the AWS profile information from configuration providers
            AWSOptions awsOptions = configuration.GetAWSOptions();

            // Configure AWS service clients to use these credentials
            services.AddDefaultAWSOptions(awsOptions);

            // Note: Both IAmazonDynamoDB and DynamoDBContext are thread safe. so it is a good idea to construct it once per application and reuse it.
            // DynamoDBContext: https://aws.amazon.com/articles/using-amazon-dynamodb-object-persistence-framework-an-introduction/
            // IAmazonDynamoDB: https://stackoverflow.com/a/56863824

            var dynamoDbConfig = configuration.GetSection(DYNAMODB_SECTION);
            var runLocalDynamoDb = dynamoDbConfig?.GetValue<bool>(DYNAMODB_SECTION_LOCALMODE) ?? false;

            if (runLocalDynamoDb)
            {
                services.AddSingleton<IAmazonDynamoDB>(sp =>
                {
                    var clientConfig = new AmazonDynamoDBConfig { ServiceURL = dynamoDbConfig.GetValue<string>(DYNAMODB_SECTION_LOCALSERVICEURL) };
                    return new AmazonDynamoDBClient(clientConfig);
                });
            }
            else
            {
                services.AddAWSService<IAmazonDynamoDB>();
            }


            tableNamePrefix = tableNamePrefixParam;

            if (string.IsNullOrWhiteSpace(tableNamePrefix))
            {
                services.AddSingleton<IDynamoDBContext, DynamoDBContext>();
            }
            else
            {
                services.AddSingleton<IDynamoDBContext, DynamoDBContext>((serviceProvider) =>
                {
                    IAmazonDynamoDB amazonDynamoDBClient = serviceProvider.GetRequiredService<IAmazonDynamoDB>();
                    DynamoDBContextConfig dynamoDBContextConfig = new DynamoDBContextConfig
                    {
                        TableNamePrefix = tableNamePrefix
                    };
                    return new DynamoDBContext(amazonDynamoDBClient, dynamoDBContextConfig);
                });
            }

            services.AddSingleton(typeof(IDynamoDBGenericRepository<>), typeof(DynamoDBGenericRepository<>));
            services.AddSingleton<IDynamoDBRepository, DynamoDBRepository>();

            return services;
        }
    }
}
