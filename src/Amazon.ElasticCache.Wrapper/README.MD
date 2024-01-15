# Getting Started
This library is a wrapper allowing you to perform operations against Elastic Cache. It uses Microsoft.Extensions.Caching.StackExchangeRedis nuget package to interact with cache functionality. 

### 1. Setup
1. Create a new `ASP.NET Core Web API` project from Visual Studio.
2. Add project reference of `Amazon.ElastiCache.Wrapper` project. You should create a nuget package of Amazon.ElastiCache.Wrapper project once everything works as expected.
3. Go to `program.cs` file and register ElastiCache client as shown below.
    ```
    // Register ElastiCache
    // service is an object of ServiceCollection
    // RegisterElasticCacheServices is an extenstion method that injects all the dependencies required for ElastiCache wrapper to work

    service.RegisterElasticCacheServices(builder.Configuration);

    ```
    Here `bucketName` comes from configuration that could be `appsettings.json` file or Environment variable or System Manager Parameter store. Region of ElastiCache comes from AWS project such as mentioned below.
    ```
    {
      "AWS": {
        "Profile": "local-test-profile",
        "Region": "us-west-2"
      }
    }
    ```  
    Also, add below settings in `appsettings.json` file
    ```
    "ElastiCache": {
        "InstanceName": "SampleInstance",
        "ServerEndpoint": "localhost:6379"
    }
    ```  
    That's all the setup required to consume the ElastiCache wrapper library.

### 2. Calling ElastiCache wrapper methods
1. To use the ElastiCache library in the code, you should inject the `IRedisCacheRepository` interface that provides the implementation of the following methods:

    a) GetStringAsync // To fetch string from cache.
    b) SetStringAsync // To set string to cache
    c) RemoveAsync // Remove cache

2. Once the `IRedisCacheRepository` is injected, you can call the above methods with appropriate inputs.

## 3. About the Project code

a) Configuration folder contains a class named ElasticCacheOptions which defines the configuration requires for Cache to work.

b) Extensions folder contains StartupExtentions class which has the method to register the dependencies for Redis Cache.

c) IRedisCacheRepository interface and RedisCacheRepository class is at the root level of Amazon.ElastiCache.Wrapper folder. IRedisCacheRepository exposes the methods and RedisCacheRepository implements the functionality IRedisCacheRepository exposes.
 
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
                "ElastiCache:PutObject",
                "ElastiCache:GetObject",
                "ElastiCache:AbortMultipartUpload",
                "ElastiCache:GetObjectAttributes",
                "ElastiCache:DeleteObject"
            ],
            "Resource": "arn:aws:ElastiCache:::<BucketName>/*"
        }
    ]
}
```
