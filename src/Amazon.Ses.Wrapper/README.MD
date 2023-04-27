# Getting Started
This library is a wrapper allowing you to perform operations against Simple Email Service (SES). It uses multiple Amazon.SimpleEmail nuget packages to provide functionality to send email, send email with attachment.

### 1. Setup
1. Create a new `ASP.NET Core Web API` project from Visual Studio.
2. Add project reference of `Amazon.Ses.Wrapper` project. You should create a nuget package of Amazon.Ses.Wrapper project once everything works as expected.
3. Go to `program.cs` file and register SES service as shown below.
    ```
    // service is an object of ServiceCollection
    // RegisterSesServices is an extenstion method that injects all the dependencies required for SES wrapper to work.

    var builder = WebApplication.CreateBuilder(args);
    service.RegisterSesServices(builder.Configuration, "SESOptions");

    ```

    First argument to `RegisterSesServices` is IConfiguration and 2nd parameter is the string which has the SESOptions defined as per the below json:
    
    Region, MaxNumberOfMessages and WaitTimeSeconds are optional. Default region would be taken if there is none specified in the configuration either via SESOptions or via AWS section (shown below)

    ```

     "SESOptions" : {
        "Region": "region",
        "AssumeRoleFlag": true,
        "AssumeRoleArn": "region",
        "AssumeRoleSessionName": "",
        "Sender": ""
    }

    {
      "AWS": {
        "Profile": "local-test-profile",
        "Region": "us-west-2"
      }
    }
    ```  
    That's all the setup required to consume the SES wrapper library.

### 2. Calling SES wrapper methods
1. To use the SES library in the code, you should inject the `ISESEmailService` interface that provides the implementation of the following methods:

    - `SendEmailAsync` Sends an email to the recipient without an attachment.
    - `SendEmailWithAttachmentAsync` Sends an email to the recipient with an attachment using the attachment as a file path.
    - `SendEmailWithAttachmentAsync` Sends an email to the recipient with an attachment using the stream being passed as an attachment.

2. Once the `ISESEmailService` is injected, you can call the above methods with appropriate inputs.

## 3. About the Project code

a) Configuration folder contains a class named SESOptions which defines the configuration requires for SES to work.

b) Extensions folder contains SesStartupExtension class which has the method to register the dependencies for SES Client.

c) Interfaces folder contains interface (ISESEmailService) for message to provide generic message format for SES and methods exposed for SES Wrapper which is in ISESEmailService interface.

d) Implementations folder contains the implementation of ISESEmailService to provide the functionality of sending emails via SES with or without attachment.

## 4. IAM Permissions required for the operations

a) IAM Permission required for all the operations are:

```
{
    "Version": "2012-10-17",
    "Statement": [
        {
            "Sid": "VisualEditor0",
            "Effect": "Allow",
            "Action": [
                "ses:SendEmail",
                "ses:SendRawEmail"
            ],
            "Resource": [
                "arn:aws:ses:*:<AWSAccount>:configuration-set/*",
                "arn:aws:ses:*:<AWSAccount>:identity/*"
            ]
        }
    ]
}
```

The resources should be restricted further to a particular configuration set of SES and identity of an email address/domain.