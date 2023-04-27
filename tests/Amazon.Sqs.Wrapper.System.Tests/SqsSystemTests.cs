using Xunit;

namespace Amazon.Sqs.Wrapper.System.Tests
{

    public class SqsSystemTests : SystemTestBase
    {
        [Fact]
        public async Task CreateQueueIsSuccessful()
        {
            //arrange
            var sqsQueue = new SqsMessageQueue(SqsClient, SqsConfig);
            //act
            var response = await sqsQueue.CreateQueueAsync(SqsConfig.Value.QueueName);
            await sqsQueue.DeleteQueueAsync(SqsConfig.Value.QueueUrl);
            //assert
            Assert.Equal(200, response);
        }

        [Fact]
        public async Task SendMessageIsSuccessful()
        {
            //arrange
            var testMesssage = "This is a test message";
            var sqsQueue = new SqsMessageQueue(SqsClient, SqsConfig);
            await sqsQueue.CreateQueueAsync(SqsConfig.Value.QueueName);
            //act
            var response = await sqsQueue.SendMessageAsync(testMesssage);
            await sqsQueue.DeleteQueueAsync(SqsConfig.Value.QueueUrl);
            //assert
            Assert.NotNull(response.Item1);
            Assert.Equal(200, response.Item2);
        }

        [Fact]
        public async Task ReceiveMessageIsSuccessful()
        {
            //arrange
            var testMesssage = "This is a test message";
            var sqsQueue = new SqsMessageQueue(SqsClient, SqsConfig);
            await sqsQueue.CreateQueueAsync(SqsConfig.Value.QueueName);
            await sqsQueue.SendMessageAsync(testMesssage);
            //act
            var response = await sqsQueue.ReceiveMessageAsync();
            var receiptHandle = response[0].Id;
            await sqsQueue.DeleteMessageAsync(receiptHandle);
            await sqsQueue.DeleteQueueAsync(SqsConfig.Value.QueueUrl);
            //assert
            Assert.NotNull(response);
            Assert.Equal(1, response.Count);
        }

        [Fact]
        public async Task DeleteMessageIsSuccessful()
        {
            //arrange
            var testMesssage = "This is a test message";
            var sqsQueue = new SqsMessageQueue(SqsClient, SqsConfig);
            await sqsQueue.CreateQueueAsync(SqsConfig.Value.QueueName);
            await sqsQueue.SendMessageAsync(testMesssage);
            //act
            var receivedMessage = await sqsQueue.ReceiveMessageAsync();
            var receiptHandle = receivedMessage[0].Id;
            var response = await sqsQueue.DeleteMessageAsync(receiptHandle);
            await sqsQueue.DeleteQueueAsync(SqsConfig.Value.QueueUrl);
            //assert
            Assert.Equal(200, response);
        }

        [Fact]
        public async Task DeleteQueueIsSuccessful()
        {
            //arrange
            var sqsQueue = new SqsMessageQueue(SqsClient, SqsConfig);
            await sqsQueue.CreateQueueAsync(SqsConfig.Value.QueueName);
            //act
            var response = await sqsQueue.DeleteQueueAsync(SqsConfig.Value.QueueUrl);
            //assert
            Assert.Equal(200, response);
        }
    }
}