using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Amazon.Sqs.Wrapper.Interfaces
{
    /// <summary>
    /// Interface to provide SQS Operations
    /// </summary>
    public interface IMessageQueue
    {
        /// <summary>
        /// Creates a Amazon Simple Queue Service (SQS) Queue.
        /// </summary>
        /// <param name="queueName">Name of SQS Queue</param>
        /// <param name="cancellationToken"> A cancellation token that can be used by other objects or threads to receive
        ///     notice of cancellation.</param>
        /// <returns>Returns an int containing HttpStatusCode returned from Amazon SQS</returns>
        Task<int> CreateQueueAsync(string queueName, CancellationToken ct = default);
        
        /// <summary>
        /// Deletes a Amazon Simple Queue Service (SQS) Queue.
        /// </summary>
        /// <param name="queueName">Name of SQS Queue</param>
        /// <param name="cancellationToken"> A cancellation token that can be used by other objects or threads to receive
        ///     notice of cancellation.</param>
        /// <returns>Returns an int containing HttpStatusCode returned from Amazon SQS</returns>
        Task<int> DeleteQueueAsync(string queueName, CancellationToken ct = default);
        
        /// <summary>
        /// Sends a message to Amazon Simple Queue Service (SQS) Queue.
        /// </summary>
        /// <param name="message">Generic message type containing message to be sent to SQS</param>
        /// <param name="queueUrl">Optional queueUrl</param>
        /// <param name="cancellationToken"> A cancellation token that can be used by other objects or threads to receive
        ///     notice of cancellation.</param>
        /// <returns>Returns a tuple containing message id and int containing HttpStatusCode returned from Amazon SQS</returns>
        Task<(string, int)> SendMessageAsync<TMessage>(TMessage message,string queueUrl = default, CancellationToken ct = default);
        
        /// <summary>
        /// Sends a message to Amazon Simple Queue Service (SQS) Queue.
        /// </summary>
        /// <param name="message">Generic message type containing message to be sent to SQS</param>
        /// <param name="messageGroupId">Message Group Id for FIFO queues</param>
        /// <param name="deduplicationId">deduplicationId for FIFO queues</param>
        /// <param name="cancellationToken"> A cancellation token that can be used by other objects or threads to receive
        ///     notice of cancellation.</param>
        /// <returns>Returns a tuple containing message id and int containing HttpStatusCode returned from Amazon SQS</returns>
        Task<(string, int)> SendMessageAsync<TMessage>(TMessage message, string messageGroupId, string deduplicationId = default, CancellationToken ct = default);

        /// <summary>
        /// Receive one or more messages from Amazon Simple Queue Service (SQS) Queue.
        /// </summary>
        /// <param name="messageProcessor">Delegate to process the message once its received</param>
        /// <param name="cancellationToken"> A cancellation token that can be used by other objects or threads to receive
        ///     notice of cancellation.</param>
        Task ReceiveMessageAsync(Func<IList<IMessage>, Task> messageProcessor, CancellationToken ct = default);

        /// <summary>
        /// Receive one or more messages from Amazon Simple Queue Service (SQS) Queue.
        /// </summary>
        /// <param name="cancellationToken"> A cancellation token that can be used by other objects or threads to receive
        ///     notice of cancellation.</param>
        /// <returns>Returns a list of IMessage received from Amazon SQS</returns>
        Task<IList<IMessage>> ReceiveMessageAsync(CancellationToken ct = default);
        
        /// <summary>
        /// Deletes a Amazon Simple Queue Service (SQS) Queue.
        /// </summary>
        /// <param name="receiptHandle">receiptHandle of message to be deleted from SQS Queue</param>
        /// <param name="cancellationToken"> A cancellation token that can be used by other objects or threads to receive
        ///     notice of cancellation.</param>
        /// <returns>Returns an int containing HttpStatusCode returned from Amazon SQS</returns>
        Task<int> DeleteMessageAsync(string receiptHandle, CancellationToken ct = default);
    }
}
