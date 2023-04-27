# Getting Started
This library is a wrapper allowing you to fetch secrets from secret manager. It uses Amazon.SecretsManager nuget packages.

### 1. Setup

1. Create a new `ASP.NET Core Web API` project from Visual Studio.
2. Add project reference of `Amazon.SecretManager.Wrapper` project.
3. Go to `program.cs` file and register Secrets Manager services like below.
    ```
    // /dev/secrets is the secret name you want to fetch from secret manager.

    var configuration = new ConfigurationBuilder()
                        .AddSecrets("/dev/secrets")
                        .Build();
    ```
    Here region is going to come from `Configuration` which could be defined in `appsettings.json` file (if defined). There is a function overload of AddSecrets where region can be provided as input.
    ```
    {
      "AWS": {
        "Profile": "local-test-profile",
        "Region": "us-west-2"
      }
    }
    ```  

    ```
     // Multiple Configuration providers can be chained in the following way:

     // Setup configuration
            var baseConfiguration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                   .AddEnvironmentVariables()
                   .AddSystemsManager($"{Environment.GetEnvironmentVariable("SSMParameterStorePrefix")}")
                   .Build();
    
    // DatabaseSecretName should be available in one of the Configuration Provider above such as appsettings.json or environment variable or SSM

            var configuration = new ConfigurationBuilder()
                        .AddConfiguration(baseConfiguration)
                        .AddSecrets(baseConfiguration["DatabaseSecretName"])
                        .Build();

    ```

## 2. About the Project code

There is no need to call any method as the dependency injection takes care of called the Build method defined in SecretManagerProvider class and Load method is override so that gets called which calls the LoadConfiguration method to load the secrets from secret manager.

## 3. IAM Permissions required for the operations

a) IAM Permission required for all the operations are:

```
{
    "Version": "2012-10-17",
    "Statement": [
        {
            "Sid": "VisualEditor0",
            "Effect": "Allow",
            "Action": [
                "secretsmanager:GetSecretValue"
            ],
            "Resource": [
                "arn:aws:secretsmanager:<Region>:<AWSAccount>:secret:<SecretId>"
            ]
        }
    ]
}
```

b) The following permissions are required when using KMS with Customer Managed Key(CMK).

```
{
    "Version": "2012-10-17",
    "Statement": [
        {
            "Sid": "VisualEditor0",
            "Effect": "Allow",
            "Action": [
                "kms:Decrypt",
                "kms:GenerateDataKey"
            ],
            "Resource": [
                "arn:aws:secretsmanager:<Region>:<AWSAccount>:secret:<SecretId>"
            ]
        }
    ]
}
```