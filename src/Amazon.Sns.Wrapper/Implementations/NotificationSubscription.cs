using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.Sns.Wrapper.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Amazon.Sns.Wrapper.Implementations
{
    public class NotificationSubscription : INotificationSubscription
    {
        private readonly IAmazonSimpleNotificationService _client;
        internal NotificationSubscription(string subscriptionArn, IAmazonSimpleNotificationService client)
        {
            Arn = subscriptionArn;
            _client = client;
        }

        internal NotificationSubscription(string subscriptionArn, string endpoint, string protocol, IAmazonSimpleNotificationService client)
        {
            Arn = subscriptionArn;
            Endpoint = endpoint;
            Protocol = protocol;
            _client = client;
        }

        public string Arn { get; private set; }
        public string Endpoint { get; private set; }
        public string Protocol { get; private set; }

        public async Task<bool> UnsubscribeAsync(CancellationToken cancellationToken = default)
        {
            var response = await _client.UnsubscribeAsync(this.Arn, cancellationToken);
            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        public async Task<Dictionary<string, string>> GetAttributesAsync(CancellationToken cancellationToken = default)
        {
            var request = new GetSubscriptionAttributesRequest { SubscriptionArn = Arn };
            var response = await _client.GetSubscriptionAttributesAsync(request, cancellationToken);

            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                throw new Exception($"Unable to get attributes for a Subscription : {Arn}");

            return response.Attributes;
        }

        public async Task<bool> AddAtrributeAsync(string attributeName, string attributeValue, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(attributeName))
                throw new ArgumentNullException($"Attribute Name cannot be null for the SNS Subscription : {Arn}");

            if (string.IsNullOrEmpty(attributeValue))
                throw new ArgumentNullException($"Attribute Value cannot be null for the SNS Subscription : {Arn}");

            var request = new SetSubscriptionAttributesRequest { SubscriptionArn = Arn, AttributeName = attributeName, AttributeValue = attributeValue };
            var response = await _client.SetSubscriptionAttributesAsync(request, cancellationToken);

            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                throw new Exception($"Unable to Set/Add Attribute to SNS Subscription :{Arn}");

            return true;
        }
    }
}
