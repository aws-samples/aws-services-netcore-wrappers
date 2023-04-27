using Amazon.Sns.Wrapper.Implementations;
using Amazon.Sns.Wrapper.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Amazon.Sns.Wrapper.Interfaces
{
    /// <summary>
    /// Interface for NotificationTopic class
    /// </summary>
    public interface INotificationTopic
    {
        /// <summary>
        /// Gets Arn of SNS Topic
        /// </summary>
        string Arn { get; }

        /// <summary>
        /// Gets Name of SNS Topic
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Adds a statement to a topic's access control policy, granting access for the
        ///     specified accounts to the specified actions.
        /// </summary>
        /// <param name="label"> A unique identifier for the new policy statement.</param>
        /// <param name="awsAccountId">The account IDs of the users (principals) who will be given access to the specified
        ///     actions. The users must have account, but do not need to be signed up for this
        ///     service.</param>
        /// <param name="actionName">The action you want to allow for the specified principal(s). Valid values: Any
        ///     Amazon SNS action name, for example
        ///     Publish</param>
        /// <param name="cancellationToken"> A cancellation token that can be used by other objects or threads to receive
        ///     notice of cancellation.</param>
        /// <returns>Returns true if the HTTP Response Status is OK</returns>
        Task<bool> AddPermissionAsync(string label, List<string> awsAccountId, List<string> actionName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes a statement from a topic's access control policy.
        /// </summary>
        /// <param name="label"> The unique label of the statement you want to remove.</param>
        /// <param name="cancellationToken"> A cancellation token that can be used by other objects or threads to receive
        ///     notice of cancellation.</param>
        /// <returns>Returns true if the HTTP Response Status is OK</returns>
        Task<bool> RemovePermissionAsync(string label, CancellationToken cancellationToken = default);

        /// <summary>
        /// -Returns all of the properties of a topic. Topic properties returned might differ
        ///     based on the authorization of the user.
        /// </summary>
        /// <param name="cancellationToken"> A cancellation token that can be used by other objects or threads to receive
        ///     notice of cancellation.</param>
        /// <returns>Properties/Atributes of SNS Topic</returns>
        Task<Dictionary<string, string>> GetAttributesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        ///     Subscribes an endpoint to an Amazon SNS topic. If the endpoint type is HTTP/S
        ///     or email, or if the endpoint and the topic are not in the same account, the endpoint
        ///     owner must run the
        ///     ConfirmSubscription
        ///     action to confirm the subscription.
        ///     You call the
        ///     ConfirmSubscription
        ///     action with the token from the subscription response. Confirmation tokens are
        ///     valid for three days.
        ///    This action is throttled at 100 transactions per second (TPS).
        /// </summary>
        /// <param name="protocol">The protocol that you want to use. Supported protocols include:
        ///     http
        ///     – delivery of JSON-encoded message via HTTP POST
        ///     https
        ///     – delivery of JSON-encoded message via HTTPS POST
        ///     email
        ///     – delivery of message via SMTP
        ///     email-json
        ///     – delivery of JSON-encoded message via SMTP
        ///     sms
        ///     – delivery of message via SMS
        ///     sqs
        ///     – delivery of JSON-encoded message to an Amazon SQS queue
        ///     application
        ///     – delivery of JSON-encoded message to an EndpointArn for a mobile app and device
        ///     lambda
        ///     – delivery of JSON-encoded message to an Lambda function
        ///     firehose
        ///     – delivery of JSON-encoded message to an Amazon Kinesis Data Firehose delivery
        ///     stream.</param>
        /// <param name="endpoint">The endpoint that you want to receive notifications. Endpoints vary by protocol:
        ///     For the
        ///     http
        ///     protocol, the (public) endpoint is a URL beginning with
        ///     http://
        ///     . For the
        ///     https
        ///     protocol, the (public) endpoint is a URL beginning with
        ///     https://
        ///     . For the
        ///     email
        ///     protocol, the endpoint is an email address. For the
        ///    email-json
        ///     protocol, the endpoint is an email address. For the
        ///     sms
        ///     protocol, the endpoint is a phone number of an SMS-enabled device. For the
        ///     sqs
        ///     protocol, the endpoint is the ARN of an Amazon SQS queue. For the
        ///     application
        ///     protocol, the endpoint is the EndpointArn of a mobile app and device. For the
        ///     lambda
        ///     protocol, the endpoint is the ARN of an Lambda function. For the
        ///     firehose
        ///     protocol, the endpoint is the ARN of an Amazon Kinesis Data Firehose delivery
        ///     stream.</param>
        /// <param name="cancellationToken"> A cancellation token that can be used by other objects or threads to receive
        ///     notice of cancellation.</param>
        /// <returns>Returns INotificationSubscription instance of newly created SNS Subscription</returns>
        Task<INotificationSubscription> SubscribeAsync(NotificationProtocol protocol, string endpoint, CancellationToken cancellationToken = default);

        /// <summary>
        ///    Verifies an endpoint owner's intent to receive messages by validating the token
        ///     sent to the endpoint by an earlier
        ///     Subscribe
        ///     action. If the token is valid, the action creates a new subscription and returns
        ///     its Amazon Resource Name (ARN). This call requires an AWS signature only when
        ///     the
        ///     AuthenticateOnUnsubscribe
        ///     flag is set to "true".
        /// </summary>
        /// <param name="token">Short-lived token sent to an endpoint during the Subscribe action.</param>
        /// <param name="cancellationToken"> A cancellation token that can be used by other objects or threads to receive
        ///     notice of cancellation.</param>
        /// <returns>Returns INotificationSubscription instance of confirmed SNS Subscription</returns>
        Task<INotificationSubscription> ConfirmSubscriptionAsync(string token, CancellationToken cancellationToken = default);

        /// <summary>
        ///    Returns a list of the subscriptions to a specific topic. Each call returns a
        ///     limited list of subscriptions, up to 100. If there are more subscriptions, a
        ///     NextToken is also returned. Use the
        ///     MaxRecords parameter to get further results using tokens returned.
        ///     This action is throttled at 30 transactions per second (TPS).
        /// </summary>
        /// <param name="maxRecords">Maximum records to be returned (default 100)</param>
        /// <param name="cancellationToken"> A cancellation token that can be used by other objects or threads to receive
        ///     notice of cancellation.</param>
        /// <returns>List of INotificationSubscription instances of a given SNS Topic</returns>
        Task<List<INotificationSubscription>> ListSubscriptionsAsync(int maxRecords = 100, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Sends a message to an Amazon SNS topic, a text message (SMS message) directly
        ///     to a phone number, or a message to a mobile platform endpoint (when you specify
        ///     the
        ///     TargetArn
        ///     ).
        ///     If you send a message to a topic, Amazon SNS delivers the message to each endpoint
        ///     that is subscribed to the topic. The format of the message depends on the notification
        ///     protocol for each subscribed endpoint.
        ///     When a
        ///     messageId
        ///     is returned, the message has been saved and Amazon SNS will attempt to deliver
        ///     it shortly.
        ///     To use the
        ///     Publish
        ///     action for sending a message to a mobile endpoint, such as an app on a Kindle
        ///     device or mobile phone, you must specify the EndpointArn for the TargetArn parameter.
        ///     The EndpointArn is returned when making a call with the
        ///     CreatePlatformEndpoint
        ///     action.
        ///     For more information about formatting messages, see Send Custom Platform-Specific
        ///     Payloads in Messages to Mobile Devices.
        ///     You can publish messages only to topics and endpoints in the same Region.
        /// </summary>
        /// <typeparam name="T">Type of Message</typeparam>
        /// <param name="message"> The message you want to send. If you are publishing to a topic and you want to
        ///     send the same message to all transport protocols, include the text of the message
        ///     as a String value. If you want to send different messages for each transport
        ///     protocol, set the value of the
        ///     MessageStructure
        ///     parameter to
        ///     json
        ///     and use a JSON object for the
        ///     Message
        ///     parameter. Constraints: With the exception of SMS, messages must be UTF-8 encoded
        ///     strings and at most 256 KB in size (262,144 bytes, not 262,144 characters). For
        ///     SMS, each message can contain up to 140 characters. This character limit depends
        ///     on the encoding schema. For example, an SMS message can contain 160 GSM characters,
        ///     140 ASCII characters, or 70 UCS-2 characters. If you publish a message that exceeds
        ///     this size limit, Amazon SNS sends the message as multiple messages, each fitting
        ///     within the size limit. Messages aren't truncated mid-word but are cut off at
        ///     whole-word boundaries. The total size limit for a single SMS
        ///     Publish
        ///     action is 1,600 characters. JSON-specific constraints: Keys in the JSON object
        ///     that correspond to supported transport protocols must have simple JSON string
        ///     values. The values will be parsed (unescaped) before they are used in outgoing
        ///     messages. Outbound notifications are JSON encoded (meaning that the characters
        ///     will be reescaped for sending). Values have a minimum length of 0 (the empty
        ///     string, "", is allowed). Values have a maximum length bounded by the overall
        ///     message size (so, including multiple protocols may limit message sizes). Non-string
        ///     values will cause the key to be ignored. Keys that do not correspond to supported
        ///     transport protocols are ignored. Duplicate keys are not allowed. Failure to parse
        ///     or validate any key or value in the message will cause the
        ///     Publish
        ///     call to return an error (no partial delivery).</param>
        /// <param name="cancellationToken"> A cancellation token that can be used by other objects or threads to receive
        ///     notice of cancellation.</param>
        /// <returns>Returns SNS messageId for the message published</returns>
        Task<string> PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// Deletes a topic and all its subscriptions. Deleting a topic might prevent some
        ///     messages previously sent to the topic from being delivered to subscribers. This
        ///     action is idempotent, so deleting a topic that does not exist does not result
        ///     in an error.
        /// </summary>
        /// <param name="cancellationToken"> A cancellation token that can be used by other objects or threads to receive
        ///     notice of cancellation.</param>
        /// <returns>returns true if the Topic is deleted sucessfully</returns>
        Task<bool> DeleteAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Allows a topic owner to set an attribute of the topic to a new value.
        /// </summary>
        /// <param name="attributeName">A map of attributes with their corresponding values. The following lists the
        ///     names, descriptions, and values of the special request parameters that the
        ///     SetTopicAttributes
        ///     action uses:
        ///     DeliveryPolicy
        ///     – The policy that defines how Amazon SNS retries failed deliveries to HTTP/S
        ///     endpoints.
        ///     DisplayName
        ///     – The display name to use for a topic with SMS subscriptions.
        ///     Policy
        ///     – The policy that defines who can access your topic. By default, only the topic
        ///     owner can publish or subscribe to the topic. The following attribute applies
        ///     only to server-side-encryption:
        ///     KmsMasterKeyId
        ///     – The ID of an Amazon Web Services managed customer master key (CMK) for Amazon
        ///     SNS or a custom CMK. For more information, see Key Terms. For more examples,
        ///     see KeyId in the Key Management Service API Reference. The following attribute
        ///     applies only to FIFO topics:
        ///     ContentBasedDeduplication
        ///     – Enables content-based deduplication for FIFO topics. By default,
        ///     ContentBasedDeduplication
        ///     is set to
        ///     false
        ///     . If you create a FIFO topic and this attribute is
        ///     false
        ///     , you must specify a value for the
        ///     MessageDeduplicationId
        ///     parameter for the Publish action. When you set
        ///     ContentBasedDeduplication
        ///     to
        ///     true
        ///     , Amazon SNS uses a SHA-256 hash to generate the
        ///     MessageDeduplicationId
        ///     using the body of the message (but not the attributes of the message). (Optional)
        ///     To override the generated value, you can specify a value for the
        ///     MessageDeduplicationId
        ///     parameter for the Publish action.</param>
        /// <param name="attributeValue">The new value for the attribute.</param>
        /// <param name="cancellationToken"> A cancellation token that can be used by other objects or threads to receive
        ///     notice of cancellation.</param>
        /// <returns></returns>
        Task<bool> AddAttributeAsync(string attributeName, string attributeValue, CancellationToken cancellationToken = default);
    }
}
