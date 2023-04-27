using Amazon.SimpleNotificationService.Model;
using Amazon.Sns.Wrapper.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Amazon.Sns.Wrapper.Interfaces
{
    public interface IMobilePushNotification
    {
        /// <summary>
        /// Creates a platform application object for one of the supported push notification
        /// services, such as APNS and GCM (Firebase Cloud Messaging), to which devices and
        /// mobile apps may register. 
        /// </summary>
        /// <param name="name">Name of Platform Application</param>
        /// <param name="platform">Platform type</param>
        /// <param name="attributes">Properties/Attributes of Platform Application</param>
        /// <param name="cancellationToken"> A cancellation token that can be used by other objects or threads to receive
        ///     notice of cancellation.</param>
        /// <returns>Returns Platform Application Arn</returns>
        Task<string> CreatePlatformApplicationAsync(string name, NotificationPlatform platform, Dictionary<string, string> attributes, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Creates an endpoint for a device and mobile app on one of the supported push
        ///     notification services, such as GCM (Firebase Cloud Messaging) and APNS.
        ///     CreatePlatformEndpoint
        ///     requires the
        ///     PlatformApplicationArn
        ///     that is returned from
        ///     CreatePlatformApplication
        ///     . You can use the returned
        ///     EndpointArn
        ///     to send a message to a mobile app or by the
        ///     Subscribe
        ///     action for subscription to a topic. The
        ///     CreatePlatformEndpoint
        ///     action is idempotent, so if the requester already owns an endpoint with the same
        ///     device token and attributes, that endpoint's ARN is returned without creating
        ///     a new endpoint. For more information, see Using Amazon SNS Mobile Push Notifications.
        ///     When using
        ///     CreatePlatformEndpoint
        ///     with Baidu, two attributes must be provided: ChannelId and UserId. The token
        ///     field must also contain the ChannelId. For more information, see Creating an
        ///     Amazon SNS Endpoint for Baidu.
        /// </summary>
        /// <param name="platformApplicationArn">Platform Application Arn</param>
        /// <param name="token">Token</param>
        /// <param name="description">Description</param>
        /// <param name="attributes">Attributes for an Endpoint</param>
        /// <param name="cancellationToken"> A cancellation token that can be used by other objects or threads to receive
        ///     notice of cancellation.</param>
        /// <returns>Returns Platform Endpoint Arn</returns>
        Task<string> CreatePlatformEndpointAsync(string platformApplicationArn, string token, string description, Dictionary<string, string> attributes, CancellationToken cancellationToken = default);

        /// <summary>
        ///   Deletes a platform application object for one of the supported push notification
        ///     services, such as APNS and GCM (Firebase Cloud Messaging). For more information,
        ///     see Using Amazon SNS Mobile Push Notifications.
        /// </summary>
        /// <param name="platformApplicationArn">Platform Application Arn</param>
        /// <param name="cancellationToken"> A cancellation token that can be used by other objects or threads to receive
        ///     notice of cancellation.</param>
        /// <returns>Returns true if Platform Application is delerted sucessfully</returns>
        Task<bool> DeletePlatformApplicationAsync(string platformApplicationArn, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Deletes the endpoint for a device and mobile app from Amazon SNS. This action
        ///     is idempotent. For more information, see Using Amazon SNS Mobile Push Notifications.
        ///     When you delete an endpoint that is also subscribed to a topic, then you must
        ///     also unsubscribe the endpoint from the topic.
        /// </summary>
        /// <param name="endpointArn">Platform Endpoint Arn</param>
        /// <param name="cancellationToken"> A cancellation token that can be used by other objects or threads to receive
        ///     notice of cancellation.</param>
        /// <returns>Returns true if Platform Endpoint is delerted sucessfully</returns>
        Task<bool> DeleteEndpointAsync(string endpointArn, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Retrieves the attributes of the platform application object for the supported
        ///     push notification services, such as APNS and GCM (Firebase Cloud Messaging).
        ///     For more information, see Using Amazon SNS Mobile Push Notifications.
        /// </summary>
        /// <param name="platformApplicationArn">Platform Application Arn</param>
        /// <param name="cancellationToken"> A cancellation token that can be used by other objects or threads to receive
        ///     notice of cancellation.</param>
        /// <returns>list of Properties/Attributes of Platform Application</returns>
        Task<Dictionary<string, string>> GetPlatformApplicationAttributesAsync(string platformApplicationArn, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Retrieves the endpoint attributes for a device on one of the supported push notification
        ///     services, such as GCM (Firebase Cloud Messaging) and APNS. For more information,
        ///     see Using Amazon SNS Mobile Push Notifications.
        /// </summary>
        /// <param name="endpointArn">Platform Endpoint Arn</param>
        /// <param name="cancellationToken"> A cancellation token that can be used by other objects or threads to receive
        ///     notice of cancellation.</param>
        /// <returns>list of Properties/Attributes of Platform Endpoint</returns>
        Task<Dictionary<string, string>> GetEndpointAttributesAsync(string endpointArn, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the attributes of the platform application object for the supported push
        ///     notification services, such as APNS and GCM (Firebase Cloud Messaging). For more
        ///     information, see Using Amazon SNS Mobile Push Notifications. For information
        ///     on configuring attributes for message delivery status, see Using Amazon SNS Application
        ///     Attributes for Message Delivery Status.
        /// </summary>
        /// <param name="platformApplicationArn">Platform Aplication Arn</param>
        /// <param name="attributes">Properties or Attributes to be attached</param>
        /// <param name="cancellationToken"> A cancellation token that can be used by other objects or threads to receive
        ///     notice of cancellation.</param>
        /// <returns>Returns true if the attributes are set sucessfully to platform Application</returns>
        Task<bool> SetPlatformApplicationAttributesAsync(string platformApplicationArn, Dictionary<string, string> attributes, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the attributes for an endpoint for a device on one of the supported push
        ///     notification services, such as GCM (Firebase Cloud Messaging) and APNS. For more
        ///     information, see Using Amazon SNS Mobile Push Notifications.
        /// </summary>
        /// <param name="endpointArn">Platform Endpoint Arn</param>
        /// <param name="attributes">Properties or Attributes</param>
        /// <param name="cancellationToken"> A cancellation token that can be used by other objects or threads to receive
        ///     notice of cancellation.</param>
        /// <returns>Returns true if the attributes are set sucessfully to platform Endpoint</returns>
        Task<bool> SetEndpointAttributesAsync(string endpointArn, Dictionary<string, string> attributes, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Lists the platform application objects for the supported push notification services,
        ///     such as APNS and GCM (Firebase Cloud Messaging). The results for
        ///     ListPlatformApplications
        ///     are paginated and return a limited list of applications, up to 100. If additional
        ///     records are available after the first page results, then a NextToken string will
        ///     be returned. To receive the next pages as well, specify maxrecords parameter
        ///     For more information, see Using Amazon SNS Mobile Push Notifications.
        ///     This action is throttled at 15 transactions per second (TPS).
        /// </summary>
        /// <param name="maxRecords">Maximum Records to be returned</param>
        /// <param name="cancellationToken"> A cancellation token that can be used by other objects or threads to receive
        ///     notice of cancellation.</param>
        /// <returns>List of Platform Applications</returns>
        Task<List<PlatformApplication>> ListPlatformApplicationsAsync(int maxRecords = 100, CancellationToken cancellationToken = default);

        /// <summary>
        /// Lists the endpoints and endpoint attributes for devices in a supported push notification
        ///     service, such as GCM (Firebase Cloud Messaging) and APNS
        /// ListEndpointsByPlatformApplication
        ///     are paginated and return a limited list of applications, up to 100. If additional
        ///     records are available after the first page results, then a NextToken string will
        ///     be returned. To receive the next pages as well, specify maxrecords parameter
        ///     For more information, see Using Amazon SNS Mobile Push Notifications.
        ///     This action is throttled at 15 transactions per second (TPS).
        /// </summary>
        /// <param name="platformApplicationArn">Platform Application Arn</param>
        /// <param name="maxRecords">Max Records</param>
        /// <param name="cancellationToken"> A cancellation token that can be used by other objects or threads to receive
        ///     notice of cancellation.</param>
        /// <returns>List of Platform Endpoints of an application</returns>
        Task<List<Endpoint>> ListEndpointsByPlatformApplicationAsync(string platformApplicationArn, int maxRecords = 100, CancellationToken cancellationToken = default);
    }
}
