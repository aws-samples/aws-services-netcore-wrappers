## aws-services-netcore-wrappers

This is a AWS Services wrapper project that offers functionality to use various AWS Services in an simpler way.


It is advised to package each library as a nuget package to consume it.

For quick start instructions, please refer to below table which has link to individual library.

| Name                                                                  | Description                                                                                                           |
|-----------------------------------------------------------------------|-----------------------------------------------------------------------------------------------------------------------|
| [Amazon.DynamoDb.Wrapper](src/Amazon.DynamoDb.Wrapper/README.MD)                                   | This library is an implementation of repository pattern in C# to connect .NET applications with DynamoDB.      |
| [Amazon.S3.Wrapper](src/Amazon.S3.Wrapper/README.MD)                           | This library is a wrapper allows you to perform operations against S3 storage. It uses multiple Amazon.S3 nuget package and makes use of Amazon.S3.Transfer nuget package to provide multi-part upload functionality.                                                                  |
| [Amazon.SecretManager.Wrapper](src/Amazon.SecretManager.Wrapper/README.MD) | This library is a wrapper that allows you to fetch secrets from secret manager. It uses Amazon.SecretsManager nuget package.                         |
| [Amazon.Ses.Wrapper](src/Amazon.Ses.Wrapper/README.MD)       | This library is a wrapper that allows you to perform operations against Simple Email Service (SES). It uses multiple Amazon.SimpleEmail nuget package to provide functionality to send email with or without attachment.                                                      |
| [Amazon.Sns.Wrapper](src/Amazon.Sns.Wrapper/README.md) | This library is a wrapper that allows you to perform operations against Simple Notification Service (SNS). It uses multiple Amazon.SimpleNotificationService nuget package to provide functionality to create sns topic, create subscription, delete topic, delete subscription, subscribe to a topic, publish notification via different channels like mobile, email, sqs, sms, http, etc.                                                                            |
| [Amazon.Sqs.Wrapper](src/Amazon.Sqs.Wrapper/README.MD) | This library is a wrapper that allows you to perform operations against Simple Queue Service (SQS). It uses multiple Amazon.SQS nuget package to provide functionality to create queue, delete queue, send, receive and delete messages from SQS queue.       |


## Security

See [CONTRIBUTING](CONTRIBUTING.md#security-issue-notifications) for more information.

## License

This library is licensed under the MIT-0 License. See the [LICENSE](LICENSE) file.
