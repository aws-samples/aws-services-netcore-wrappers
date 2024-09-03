# Getting Started
This library is a wrapper allowing you to perform operations against Simple Queue Service (SQS). It uses multiple Amazon.SQS nuget packages to provide functionality to create queue, delete queue, send, receive and delete message from SQS queue.

### 1. Setup
1. Create a new `ASP.NET Core Web API` project from Visual Studio.
2. Add project reference of `Amazon.Sqs.Wrapper` project. You should create a nuget package of Amazon.Sqs.Wrapper project once everything works as expected.
3. Go to `program.cs` file and register SQS client as shown below.
    ```
    // Register SQS client
    // service is an object of ServiceCollection
    // RegisterSqsClient is an extenstion method that injects all the dependencies required for SQS wrapper to work.

    var builder = WebApplication.CreateBuilder(args);
    service.RegisterSqsClient(builder.Configuration, "SqsConfigSectionName");

    ```

    First argument to `RegisterSqsClient` is IConfiguration and 2nd parameter is the string which has the SqsConfig defined as per the below json:
    
    Region, MaxNumberOfMessages and WaitTimeSeconds are optional. Default region would be taken if there is none specified in the configuration either via SqsConfig or via AWS section (shown below)

    ```

     "SqsConfig" : {
        "QueueName": "SqsQueueName",
        "QueueUrl": "SqsQueueUrl",
        "Region": "region",
        "MaxNumberOfMessages": 10,
        "WaitTimeSeconds": 20
    }

    {
      "AWS": {
        "Profile": "local-test-profile",
        "Region": "us-west-2"
      }
    }
    ```  
    That's all the setup required to consume the sqs wrapper library.

### 2. Calling Sqs wrapper methods
1. To use the Sqs library in the code, you should inject the `IMessageQueue` interface that provides the implementation of the following methods:

    - `CreateQueueAsync` Creates a Queue
    - `DeleteQueueAsync` Deletes a Queue
    - `SendMessageAsync` Sends a message to SQS Queue. SendMessageAsync have two overloads - one which sends message to standard queue and another sends message to FIFO queue.
    - `ReceiveMessageAsync` Receives a message from the queue. ReceiveMessageAsync have two overloads - one which takes a delegate to specify how message needs to be processed. Other does not take any argument except optional cancellation token, so caller should process the message after response is returned from this method.
    - `DeleteMessageAsync` Deletes a message from SQS based on receiptHandle provided.
    - `StartMessageMoveTaskAsync` Initiates a task to move messages from a Dead Letter Queue (DLQ) to the main queue. This method helps in managing and recovering messages that failed to be processed.
    - `GetRedrivePolicyAsync` Retrieves the redrive policy for the specified queue, which includes the ARN of the Dead Letter Queue (DLQ) required as input for the StartMessageMoveTaskAsync method.

2. Once the `IMessageQueue` is injected, you can call the above methods with appropriate inputs.

## 3. About the Project code

a) Configuration folder contains a class named SqsConfig which defines the configuration requires for SQS to work.

b) Extensions folder contains SqsExtension class which has the method to register the dependencies for SQS Client.

c) Interfaces folder contains interface (IMessage) for message to provide generic message format for SQS and methods exposed for Sqs Wrapper which is in IMessageQueue interface.

d) At the root of Aws.Sqs.Wrapper, we have implementation of IMessage and IMessageQueue to provide the functionality of creating sqs queue, deleting sqs queue, send message to sqs, receive message from sqs and delete message from SQS queue.

e) Unit tests are written /tests/Amazon.Sqs.Wrapper.Tests project for each interface exposing a specific functionality.

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
                "sqs:DeleteMessage",
                "sqs:ReceiveMessage",
                "sqs:DeleteQueue",
                "sqs:SendMessage",
                "sqs:CreateQueue",
                "sqs:GetQueueAttributes",
                "sqs:StartMessageMoveTask"
            ],
            "Resource": "arn:aws:sqs:<Region>:<AWSAccount>:<QueueName>"
        }
    ]
}
```