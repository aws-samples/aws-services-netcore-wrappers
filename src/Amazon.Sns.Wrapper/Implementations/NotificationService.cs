using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.Sns.Wrapper.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Amazon.Sns.Wrapper.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly IAmazonSimpleNotificationService _client;
        private IMobilePushNotification _pushNotificationService = null;

        public NotificationService(IAmazonSimpleNotificationService client)
        {
            _client = client;
        }

        public IMobilePushNotification PushNotification
        {
            get
            {
                if (_pushNotificationService == null)
                    _pushNotificationService = new MobilePushNotification(_client);

                return _pushNotificationService;
            }
        }

        public async Task<INotificationTopic> CreateTopicAsync(string name, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException($"Topic Name cannot be null to create SNS Topic");

            var request = new CreateTopicRequest { Name = name };
            var response = await _client.CreateTopicAsync(request, cancellationToken);
            if (string.IsNullOrEmpty(response.TopicArn))
            {
                throw new Exception($"Unable to Create SNS Topic with name - {name}");
            }

            INotificationTopic topic = new NotificationTopic(name, response.TopicArn, _client);
            return topic;
        }

        public async Task<INotificationTopic> GetTopicAsync(string name, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException($"Topic Name cannot be null to get SNS Topic");

            var response = await _client.FindTopicAsync(name);
            if (response == null)
            {
                throw new Exception($"Unable to Get Topic SNS Instance with name - {name}");
            }

            INotificationTopic topic = new NotificationTopic(name, response.TopicArn, _client);
            return topic;
        }
    }
}
