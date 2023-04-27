# Getting Started
This library is a wrapper allowing you to perform operations against S3 storage. It uses multiple Amazon.S3 nuget packages and makes use of Amazon.S3.Transfer nuget package to provide multi-part upload functionality. 

### 1. Setup
1. Create a new `ASP.NET Core Web API` project from Visual Studio.
2. Add project reference of `Amazon.S3.Wrapper` project. You should create a nuget package of Amazon.S3.Wrapper project once everything works as expected.
3. Go to `program.cs` file and register S3 client as shown below.
    ```
    // Register S3 client
    // service is an object of ServiceCollection
    // RegisterIAmazonS3Client is an extenstion method that injects all the dependencies required for S3 wrapper to work

    service.RegisterIAmazonS3Client(new AmazonS3Config(), bucketName);

    ```
    Here `bucketName` comes from configuration that could be `appsettings.json` file or Environment variable or System Manager Parameter store. Region of S3 comes from AWS project such as mentioned below.
    ```
    {
      "AWS": {
        "Profile": "local-test-profile",
        "Region": "us-west-2"
      }
    }
    ```  
    That's all the setup required to consume the s3 wrapper library.

### 2. Calling S3 wrapper methods
1. To use the S3 library in the code, you should inject the `IS3Client` interface that provides the implementation of the following methods:

    a) UploadObjectAsync // Provides multi-part object upload implementation.
    b) PutObjectAsync // Provides object upload implementation
    c) GetObjectAsync // To fetch an object from s3
    d) DeleteObjectAsync //deletes an object
    e) IsFileExists //check if an object exists

2. Once the `IS3Client` is injected, you can call the above methods with appropriate inputs.

## 3. About the Project code

a) Configuration folder contains a static class named ConfigureService which has the RegisterIAmazonS3Client extention method to inject required dependencies.

b) Enums folder contains ContentType class which has the enum for different type of contents (more types can be added here).

c) Exceptions folder contains AmazonS3ClientException class which defines the custom exception the wrapper provides in case of any exception that comes. This custom exception can be caught by the consumer.

d) Extension folder contains couple of classes which has helper extension methods.

e) Utils folder contain constants for content type.

f) IS3Client interface and S3Client class is at the root level of Amazon.S3.Wrapper folder. IS3Client exposes the methods and S3Client implements the functionality IS3Client exposes.

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
                "s3:PutObject",
                "s3:GetObject",
                "s3:AbortMultipartUpload",
                "s3:GetObjectAttributes",
                "s3:DeleteObject"
            ],
            "Resource": "arn:aws:s3:::<BucketName>/*"
        }
    ]
}
```
