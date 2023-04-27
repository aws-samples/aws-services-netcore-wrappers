using System;
using Amazon.SQS;
using Amazon.SQS.Model;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Amazon.Sqs.Wrapper.Configuration;
using Amazon.Sqs.Wrapper.Interfaces;

namespace Amazon.Sqs.Wrapper
{
    /// <summary>
    /// Functionality of SQS Operations 
    /// </summary>
    public class SqsMessageQueue : IMessageQueue
    {
        private readonly IAmazonSQS client;
        private readonly SqsConfig config;

        public SqsMessageQueue(IAmazonSQS sqsClient, IOptions<SqsConfig> sqsConfig)
        {
            client = sqsClient;
            config = sqsConfig.Value;
        }

        /// <summary>
        /// Provide functionality of creating SQS Queue
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<int> CreateQueueAsync(string queueName, CancellationToken ct = default)
        {
            var name = string.IsNullOrWhiteSpace(queueName) ? config.QueueName : queueName;
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"Queue name cannot be empty");
            }
            var createQueueRequest = new CreateQueueRequest(name);
            var response = await client.CreateQueueAsync(createQueueRequest, ct);
            return (int)response.HttpStatusCode;
        }

        /// <summary>
        /// Provide functionality of deleting SQS Queue
        /// </summary>
        /// <param name="queueUrl"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<int> DeleteQueueAsync(string queueUrl, CancellationToken ct = default)
        {
            var url = string.IsNullOrWhiteSpace(queueUrl) ? config.QueueUrl : queueUrl;
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException($"Queue url cannot be empty");
            }
            var deleteQueueRequest = new DeleteQueueRequest(url); ;
            var response = await client.DeleteQueueAsync(deleteQueueRequest, ct);
            return (int)response.HttpStatusCode;
        }

        /// <summary>
        /// Provide functionality of sending message to SQS Queue
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="message"></param>
        /// <param name="queueUrl"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<(string, int)> SendMessageAsync<TMessage>(TMessage message, string queueUrl = default, CancellationToken ct = default)
        {
            if (message == null)
            {
                throw new ArgumentException($"Message cannot be null");
            }
            if (string.IsNullOrWhiteSpace(queueUrl))
            {
                queueUrl = config.QueueUrl;
            }
            var messageStr = JsonConvert.SerializeObject(message);
            var sendMessageRequest = new SendMessageRequest(queueUrl, messageStr);
            var response = await client.SendMessageAsync(sendMessageRequest, ct);
            return (response.MessageId, (int)response.HttpStatusCode);
        }

        /// <summary>
        /// Provide functionality of sending message to FIFO SQS Queue
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="message"></param>
        /// <param name="messageGroupId"></param>
        /// <param name="deduplicationId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<(string, int)> SendMessageAsync<TMessage>(TMessage message, string messageGroupId, string deduplicationId = default, CancellationToken ct = default)
        {
            if (message == null)
            {
                throw new ArgumentException($"Message cannot be null");
            }
            var messageStr = JsonConvert.SerializeObject(message);
            var sendMessageRequest = new SendMessageRequest(config.QueueUrl, messageStr);
            sendMessageRequest.MessageGroupId = messageGroupId;
            if (!string.IsNullOrWhiteSpace(deduplicationId))
                sendMessageRequest.MessageDeduplicationId = deduplicationId;
            var response = await client.SendMessageAsync(sendMessageRequest, ct);
            return (response.MessageId, (int)response.HttpStatusCode);
        }

        /// <summary>
        /// Provide functionality of receiving message from SQS Queue
        /// </summary>
        /// <param name="messageProcessor"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task ReceiveMessageAsync(Func<IList<IMessage>, Task> messageProcessor, CancellationToken ct = default)
        {
            if (messageProcessor == null)
            {
                throw new ArgumentException($"Message Processor cannot be null");
            }
            var messages = await ReceiveMessageAsync(ct);
            await messageProcessor(messages);
            Parallel.ForEach(messages, async (message) =>
            {
                await DeleteMessageAsync(message.Id);
            });
        }

        /// <summary>
        /// Provide functionality of receiving message from SQS Queue
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<IList<IMessage>> ReceiveMessageAsync(CancellationToken ct = default)
        {
            var receiveMessageRequest = new ReceiveMessageRequest
            {
                QueueUrl = config.QueueUrl,
                MaxNumberOfMessages = config.MaxNumberOfMessages,
                WaitTimeSeconds = config.WaitTimeSeconds
            };
            var response = await client.ReceiveMessageAsync(receiveMessageRequest, ct);
            var messages = new List<IMessage>();
            foreach (var message in response.Messages)
            {
                var item = new SqsMessage { Id = message.ReceiptHandle, Body = message.Body };
                messages.Add(item);
            }
            return messages;
        }

        /// <summary>
        /// Provide functionality of deleting SQS Queue
        /// </summary>
        /// <param name="receiptHandle"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<int> DeleteMessageAsync(string receiptHandle, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(receiptHandle))
            {
                throw new ArgumentException($"Receipt Handle cannot be empty");
            }
            var deleteMessageRequest = new DeleteMessageRequest
            {
                QueueUrl = config.QueueUrl,
                ReceiptHandle = receiptHandle
            };
            var response = await client.DeleteMessageAsync(deleteMessageRequest, ct);
            return (int)response.HttpStatusCode;
        }

    }
}
