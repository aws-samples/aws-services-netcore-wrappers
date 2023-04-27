using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Amazon.Sns.Wrapper.Interfaces
{
    /// <summary>
    /// Interface of NotificationSubscription class
    /// </summary>
    public interface INotificationSubscription
    {
        /// <summary>
        /// Gets Arn of SNS Subscription 
        /// </summary>
        string Arn { get; }

        /// <summary>
        /// Gets Endpoint of SNS Subscription 
        /// </summary>
        string Endpoint { get; }

        /// <summary>
        /// Gets Protocol of SNS Subscription 
        /// </summary>
        string Protocol { get; }

        /// <summary>
        ///    Deletes a subscription. If the subscription requires authentication for deletion,
        ///     only the owner of the subscription or the topic's owner can unsubscribe, and
        ///     an Amazon Web Services signature is required. If the
        ///     Unsubscribe
        ///    call does not require authentication and the requester is not the subscription
        ///     owner, a final cancellation message is delivered to the endpoint, so that the
        ///     endpoint owner can easily resubscribe to the topic if the
        ///     Unsubscribe
        ///     request was unintended.
        ///    This action is throttled at 100 transactions per second (TPS).
        /// </summary>
        /// <param name="cancellationToken"> A cancellation token that can be used by other objects or threads to receive
        ///    notice of cancellation.</param>
        /// <returns>Returns true if the Http response is Ok</returns>
        Task<bool> UnsubscribeAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns all of the properties of a subscription.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive
        ///     notice of cancellation.</param>
        /// <returns>Attributes/Properties of a Subscription</returns>
        Task<Dictionary<string, string>> GetAttributesAsync(CancellationToken cancellationToken = default);
        /// <summary>
        ///  Allows a subscription owner to set an attribute of the subscription to a new value.
        /// </summary>
        /// <param name="attributeName"> 
        ///     A map of attributes with their corresponding values. The following lists the
        ///     names, descriptions, and values of the special request parameters that this action
        ///     uses:
        ///     DeliveryPolicy
        ///     – The policy that defines how Amazon SNS retries failed deliveries to HTTP/S
        ///     endpoints.
        ///     FilterPolicy
        ///     – The simple JSON object that lets your subscriber receive only a subset of messages,
        ///     rather than receiving every message published to the topic.
        ///     RawMessageDelivery
        ///     – When set to
        ///     true
        ///     , enables raw message delivery to Amazon SQS or HTTP/S endpoints. This eliminates
        ///     the need for the endpoints to process JSON formatting, which is otherwise created
        ///     for Amazon SNS metadata.
        ///     RedrivePolicy
        ///     – When specified, sends undeliverable messages to the specified Amazon SQS dead-letter
        ///     queue. Messages that can't be delivered due to client errors (for example, when
        ///     the subscribed endpoint is unreachable) or server errors (for example, when the
        ///     service that powers the subscribed endpoint becomes unavailable) are held in
        ///     the dead-letter queue for further analysis or reprocessing. The following attribute
        ///     applies only to Amazon Kinesis Data Firehose delivery stream subscriptions:
        ///     SubscriptionRoleArn
        ///     – The ARN of the IAM role that has the following: Permission to write to the
        ///     Kinesis Data Firehose delivery stream Amazon SNS listed as a trusted entity Specifying
        ///     a valid ARN for this attribute is required for Kinesis Data Firehose delivery
        ///     stream subscriptions. For more information, see Fanout to Kinesis Data Firehose
        ///     delivery streams in the Amazon SNS Developer Guide.
        ///</param>
        /// <param name="attributeValue">The new value for the attribute in JSON format.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive
        ///     notice of cancellation.</param>
        /// <returns></returns>
        Task<bool> AddAtrributeAsync(string attributeName, string attributeValue, CancellationToken cancellationToken = default);
    }
}
