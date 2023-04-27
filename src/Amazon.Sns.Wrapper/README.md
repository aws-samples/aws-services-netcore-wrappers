# Getting Started
This library is a wrapper allowing you to perform operations against Simple Notification Service (SNS). It uses multiple Amazon.SimpleNotificationService nuget packages to provide functionality to create sns topic, create subscription, delete topic, delete subscription,  subscribe to a topic, publish notification via different channels like mobile, email, sqs, sms, http, etc.

### 1. Setup

1. Create a new `ASP.NET Core Web API` project from Visual Studio.
2. Add project reference of `Amazon.Sns.Library` project. You should create a nuget package of Amazon.Sns.Library project once everything works as expected.

3. Go to `program.cs` file and register Sns client as shown below.
    ```
    // Register Sns client
    // service is an object of ServiceCollection
    // AddSnsClient is an extenstion method that injects all the dependencies required for SNS wrapper to work.

    var builder = WebApplication.CreateBuilder(args);
    service.AddSnsClient(builder.Configuration, "snsConfigSectionName");

    ```

    First argument to `AddSnsClient` is IConfiguration and 2nd parameter is the string which has the SnsConfig defined as per the below json:
    
    ```

    "SnsConfig": {
        "Profile": "default",
        "Region": "ap-south-1"
    }
    ```  
    That's all the setup required to consume the sns wrapper library.

### 2. Calling Sns wrapper methods

1. To use the Sns library in the code, you should inject the `INotificationService` interface via which you get access to the functionality exposed by various interfaces mentioned in next section. Some of these are used internally.

This is the example of how it can be injected in an API controller and used in the action methods.

```
    private INotificationService _service = null;

    public HomeController(ILogger<HomeController> logger, INotificationService service, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        _service = service;
    }

    public IActionResult GetTopic()
    {
        var topic = _service.GetTopicAsync("mytesttopic").Result;
        var subscription = topic.SubscribeAsync(Library.Sns.Models.NotificationProtocol.sms, "+919999999999").Result;
        var result = topic.PublishAsync<string>("test from my topic").Result;
        return Ok(topic.ListSubscriptionsAsync().Result);
    }

```

2. Once the `INotificationService` is injected, you can call the exposed methods with appropriate inputs.

## 3. About the Project code

a) Extensions folder contains StartupExtensions class which has the method to register the dependencies for Sns Client.

b) Interfaces folder contains interfaces exposing methods to do the following:

```
// From INotificationTopic

AddPermissionAsync //Adds a statement to a topic's access control policy, granting access for the specified accounts to the specified actions.

RemovePermissionAsync //Removes a statement from a topic's access control policy.

GetAttributesAsync //Returns all of the properties of a topic.

SubscribeAsync //Subscribes an endpoint to an Amazon SNS topic.

ConfirmSubscriptionAsync //Verifies an endpoint owner's intent to receive messages by validating the token sent to the endpoint by an earlier Subscribeaction

ListSubscriptionsAsync //Returns a list of the subscriptions to a specific topic

PublishAsync //Sends a message to an Amazon SNS topic, a text message (SMS message) directly to a phone number, or a message to a mobile platform endpoint.

DeleteAsync //Deletes a topic and all its subscriptions

AddAttributeAsync // Allows a topic owner to set an attribute of the topic to a new value.

// From INotificationSubscription

UnsubscribeAsync //Deletes a subscription.

GetAttributesAsync // Returns all of the properties of a subscription.

AddAtrributeAsync //Allows a subscription owner to set an attribute of the subscription to a new value.

//From INotificationService

CreateTopicAsync //Creates new SNS Topic if not already exists

GetTopicAsync // Get Topic by name

PushNotification // Gets PushNotification Service instance to perform mobile push notifications operations

//From IMobilePushNotification

CreatePlatformApplicationAsync //Creates a platform application object for one of the supported push notification services, such as APNS and GCM (Firebase Cloud Messaging), to which devices and mobile apps may register.

CreatePlatformEndpointAsync //Creates an endpoint for a device and mobile app on one of the supported push notification services, such as GCM (Firebase Cloud Messaging) and APNS.

DeletePlatformApplicationAsync //Deletes a platform application object for one of the supported push notification services, such as APNS and GCM (Firebase Cloud Messaging).

DeleteEndpointAsync //Deletes the endpoint for a device and mobile app from Amazon SNS. This action is idempotent.

GetPlatformApplicationAttributesAsync //Retrieves the attributes of the platform application object for the supported push notification services, such as APNS and GCM (Firebase Cloud Messaging).

GetEndpointAttributesAsync //Retrieves the endpoint attributes for a device on one of the supported push notification services, such as GCM (Firebase Cloud Messaging) and APNS.

SetPlatformApplicationAttributesAsync //Sets the attributes of the platform application object for the supported push notification services, such as APNS and GCM (Firebase Cloud Messaging).

SetEndpointAttributesAsync //Sets the attributes for an endpoint for a device on one of the supported push notification services, such as GCM (Firebase Cloud Messaging) and APNS.

ListPlatformApplicationsAsync //Lists the platform application objects for the supported push notification services, such as APNS and GCM (Firebase Cloud Messaging).

ListEndpointsByPlatformApplicationAsync //Lists the endpoints and endpoint attributes for devices in a supported push notification service, such as GCM (Firebase Cloud Messaging) and APNS

```

c) Implementations folder has classes to provide impementation of the above interfaces.

d) Models folder contains NotificationPlatform which is used to determine the mobile platform being used for push notification and NotificationProtocol specify the type of protocol being used for subscription such as http, https, email, sms, sqs etc.

e) At the root of Aws.Sns.Library, NotificationInfrastructure class is there which is used by the caller to inject the IAmazonSimpleNotificationService object.

f) Unit tests are written /tests/Aws.Library.Sns.Tests project for each interface exposing a specific functionality.

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
                "sns:Publish",
                "sns:GetTopicAttributes",
                "sns:DeleteTopic",
                "sns:CreateTopic",
                "sns:SetTopicAttributes",
                "sns:Subscribe",
                "sns:ConfirmSubscription",
                "sns:AddPermission",
                "sns:RemovePermission"
            ],
            "Resource": "arn:aws:sns:<Region>:<AWSAccount>:<TopicName>"
        },
        {
            "Sid": "VisualEditor1",
            "Effect": "Allow",
            "Action": [
                "sns:DeleteEndpoint",
                "sns:SetEndpointAttributes",
                "sns:GetEndpointAttributes",
                "sns:SetSubscriptionAttributes",
                "sns:DeletePlatformApplication",
                "sns:CreatePlatformApplication",
                "sns:SetPlatformApplicationAttributes",
                "sns:GetPlatformApplicationAttributes",
                "sns:CreatePlatformEndpoint",
                "sns:Unsubscribe",
                "sns:GetSubscriptionAttributes"
            ],
            "Resource": "*"
        }
    ]
}
```

Note: You should add 'Conditions' in the second statement to meet give granular permission over the endpoints and subscription attributes, the actions selected support all resources, hence we cannot restrict the resources here.