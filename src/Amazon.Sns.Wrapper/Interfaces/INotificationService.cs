using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Amazon.Sns.Wrapper.Interfaces
{
    /// <summary>
    /// Interface of NotificationService class
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Creates new SNS Topic if not already exists
        /// </summary>
        /// <param name="name">Topic Name</param>
        /// <param name="cancellationToken">Cancellation Token (Optional)</param>
        /// <returns>INotificationTopic instance of newly created Topic</returns>
        Task<INotificationTopic> CreateTopicAsync(string name, CancellationToken cancellationToken = default);
        /// <summary>
        /// Get Topic by name
        /// </summary>
        /// <param name="name">Topic Name</param>
        /// <param name="cancellationToken"></param>
        /// <returns>INotificationTopic instance</returns>
        Task<INotificationTopic> GetTopicAsync(string name, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Gets PushNotification Service instance to perform mobile push notifications operations
        /// </summary>
        IMobilePushNotification PushNotification { get; }
    }
}
