using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.Sns.Wrapper.Interfaces;
using Amazon.Sns.Wrapper.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Amazon.Sns.Wrapper.Implementations
{
    public class NotificationTopic : INotificationTopic
    {
        private readonly IAmazonSimpleNotificationService _client;
        internal NotificationTopic(string topicName, string topicArn, IAmazonSimpleNotificationService client)
        {
            _client = client;
            Name = topicName;
            Arn = topicArn;
        }

        public string Arn { get; private set; }

        public string Name { get; private set; }

        public async Task<bool> AddPermissionAsync(string label, List<string> awsAccountId, List<string> actionName, CancellationToken cancellationToken = default)
        {
            var request = new AddPermissionRequest { TopicArn = Arn, Label = label, ActionName = actionName, AWSAccountId = awsAccountId };
            var response = await _client.AddPermissionAsync(request, cancellationToken);

            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                throw new Exception($"Unable to add Permission to a Topic with name {Name}");

            return true;
        }

        public async Task<bool> RemovePermissionAsync(string label, CancellationToken cancellationToken = default)
        {
            var request = new RemovePermissionRequest { TopicArn = Arn, Label = label };
            var response = await _client.RemovePermissionAsync(request, cancellationToken);

            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                throw new Exception($"Unable to remove Permission to a Topic with name {Name}");

            return true;
        }

        public async Task<INotificationSubscription> SubscribeAsync(NotificationProtocol protocol, string endpoint, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(protocol.ToString()))
                throw new ArgumentNullException($"Protocol cannot be null to Subscribe endpoint for the SNS Topic : {Name}");

            if (string.IsNullOrEmpty(endpoint))
                throw new ArgumentNullException($"Endpoint cannot be null to Subscribe endpoint for the SNS Topic : {Name}");

            string protocolStr = protocol.ToString();
            if (protocol == NotificationProtocol.emailJson)
                protocolStr = "email-Json";

            var request = new SubscribeRequest { TopicArn = Arn, Endpoint = endpoint, Protocol = protocolStr, ReturnSubscriptionArn = true };
            var result = await _client.SubscribeAsync(request, cancellationToken);
            return new NotificationSubscription(result.SubscriptionArn, _client);
        }

        public async Task<INotificationSubscription> ConfirmSubscriptionAsync(string token, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException($"Token cannot be null to Confirm Subscription for the SNS Topic : {Name}");

            var request = new ConfirmSubscriptionRequest { TopicArn = Arn, Token = token };
            var result = await _client.ConfirmSubscriptionAsync(request, cancellationToken);
            return new NotificationSubscription(result.SubscriptionArn, _client);
        }

        public async Task<bool> DeleteAsync(CancellationToken cancellationToken = default)
        {
            var request = new DeleteTopicRequest { TopicArn = Arn };
            var response = await _client.DeleteTopicAsync(request, cancellationToken);

            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                throw new Exception($"Unable to delete Topic :{Name}");

            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        public async Task<string> PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) where TMessage : class
        {
            if (message == null)
                throw new ArgumentNullException($"Message to be published cannot be null for the SNS Topic : {Name}");

            string messageStr;
            if (typeof(TMessage) == typeof(string))
                messageStr = message.ToString();
            else
                messageStr = JsonConvert.SerializeObject(message);

            var request = new PublishRequest { TopicArn = Arn, Message = messageStr };
            var response = await _client.PublishAsync(request, cancellationToken);

            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                throw new Exception($"Unable to Publish Notification Message :{messageStr} to Topic :{Name}");

            return response.MessageId;
        }

        public async Task<List<INotificationSubscription>> ListSubscriptionsAsync(int maxRecords = 100, CancellationToken cancellationToken = default)
        {
            int recordsToGet = maxRecords;
            var subscriptions = new List<INotificationSubscription>();
            var response = await _client.ListSubscriptionsByTopicAsync(Arn, cancellationToken);

            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                throw new Exception($"Unable to get Subscription List for Topic :{Name}");

            response.Subscriptions.ForEach(s =>
            {
                var subscription = new NotificationSubscription(s.SubscriptionArn, s.Endpoint, s.Protocol, _client);
                subscriptions.Add(subscription);
            });

            if (maxRecords <= 100)
                return subscriptions;

            recordsToGet -= 100;
            while (recordsToGet > 0)
            {
                var request = new ListSubscriptionsByTopicRequest { TopicArn = Arn, NextToken = response.NextToken };
                response = await _client.ListSubscriptionsByTopicAsync(request);
                if (response == null || response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                    throw new Exception($"Unable to get Platform Applications");

                response.Subscriptions.ForEach(s =>
                {
                    var subscription = new NotificationSubscription(s.SubscriptionArn, s.Endpoint, s.Protocol, _client);
                    subscriptions.Add(subscription);
                });
                recordsToGet -= 100;
            }

            return subscriptions;
        }

        public async Task<Dictionary<string, string>> GetAttributesAsync(CancellationToken cancellationToken = default)
        {
            var request = new GetTopicAttributesRequest { TopicArn = Arn };
            var response = await _client.GetTopicAttributesAsync(request, cancellationToken);

            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                throw new Exception($"Unable to get attributes for a topic with name : {Name}");

            return response.Attributes;
        }

        public async Task<bool> AddAttributeAsync(string attributeName, string attributeValue, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(attributeName))
                throw new ArgumentNullException($"Attribute Name cannot be null for the SNS Topic : {Name}");

            if (string.IsNullOrEmpty(attributeValue))
                throw new ArgumentNullException($"Attribute Value cannot be null for the SNS Topic : {Name}");

            var request = new SetTopicAttributesRequest { TopicArn = Arn, AttributeName = attributeName, AttributeValue = attributeValue };
            var response = await _client.SetTopicAttributesAsync(request, cancellationToken);

            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                throw new Exception($"Unable to Set/Add Attribute to Topic :{Name}");

            return true;
        }
    }
}
