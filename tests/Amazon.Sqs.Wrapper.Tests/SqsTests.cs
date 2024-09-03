using Moq;
using Xunit;
using AutoFixture;
using Amazon.SQS;
using Amazon.SQS.Model;
using Amazon.Sqs.Wrapper.Configuration;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;

namespace Amazon.Sqs.Wrapper.Tests
{
    public class SqsTests
    {
        private Mock<IAmazonSQS> _fakeSqsService;
        private Mock<IOptions<SqsConfig>> _fakeSqsConfig;
        private SqsConfig _fakeSqsConfigValues;
        private Fixture _fixture;
        public SqsTests()
        {
            _fakeSqsService = new Mock<IAmazonSQS>();
            _fakeSqsConfig = new Mock<IOptions<SqsConfig>>();
            _fakeSqsConfigValues = new SqsConfig
            {
                QueueName = "fake-sqs-queue",
                QueueUrl = "https://sqs.us-east-1.amazonaws.com/000000000000/fake-sqs-queue",
                MaxNumberOfMessages = 10,
                WaitTimeSeconds = 20,
                Region = "us-east-1"
            };
            _fixture = new Fixture();
        }

        [Fact]
        public async Task CreateQueueIsSuccessful()
        {
            //arrange
            var queueName = "fake-sqs-queue";
            var createQueueResponse = _fixture.Create<CreateQueueResponse>();
            _fakeSqsService.Setup(
                p => p.CreateQueueAsync(It.IsAny<CreateQueueRequest>(), default))
                .Returns(Task.FromResult(createQueueResponse));
            _fakeSqsConfig.Setup(c => c.Value).Returns(_fakeSqsConfigValues);
            var sqsQueue = new SqsMessageQueue(_fakeSqsService.Object, _fakeSqsConfig.Object);
            //act
            var response = await sqsQueue.CreateQueueAsync(queueName);
            //assert
            Assert.Equal((int)createQueueResponse.HttpStatusCode, response);
        }

        [Fact]
        public async Task DeleteQueueIsSuccessful()
        {
            //arrange
            var queueName = "fake-sqs-queue";
            var deleteQueueResponse = _fixture.Create<DeleteQueueResponse>();
            _fakeSqsService.Setup(
                p => p.DeleteQueueAsync(It.IsAny<DeleteQueueRequest>(), default))
                .Returns(Task.FromResult(deleteQueueResponse));
            _fakeSqsConfig.Setup(c => c.Value).Returns(_fakeSqsConfigValues);
            var sqsQueue = new SqsMessageQueue(_fakeSqsService.Object, _fakeSqsConfig.Object);
            //act
            var response = await sqsQueue.DeleteQueueAsync(queueName);
            //assert
            Assert.Equal((int)deleteQueueResponse.HttpStatusCode, response);
        }

        [Fact]
        public async Task SendMessageIsSuccessful()
        {
            //arrange
            var testMesssage = "This is a test message";
            var expectedResponse = _fixture.Create<SendMessageResponse>();
            _fakeSqsService.Setup(
                p => p.SendMessageAsync(It.IsAny<SendMessageRequest>(), default))
                .Returns(Task.FromResult(expectedResponse));
            _fakeSqsConfig.Setup(c => c.Value).Returns(_fakeSqsConfigValues);
            var sqsQueue = new SqsMessageQueue(_fakeSqsService.Object, _fakeSqsConfig.Object);
            //act
            var actualResponse = await sqsQueue.SendMessageAsync(testMesssage);
            //assert
            Assert.NotNull(actualResponse.Item1);
            Assert.Equal(expectedResponse.MessageId, actualResponse.Item1);
            Assert.Equal((int)expectedResponse.HttpStatusCode, actualResponse.Item2);
        }

        [Fact]
        public async Task FIFOQueueSendMessageIsSuccessful()
        {
            //arrange
            var testMesssage = "This is a test message";
            var messageGroupId = "Group1";
            var deduplicationId = Guid.NewGuid().ToString();
            var expectedResponse = _fixture.Create<SendMessageResponse>();
            _fakeSqsService.Setup(
                p => p.SendMessageAsync(It.IsAny<SendMessageRequest>(), default))
                .Returns(Task.FromResult(expectedResponse));
            _fakeSqsConfig.Setup(c => c.Value).Returns(_fakeSqsConfigValues);
            var sqsQueue = new SqsMessageQueue(_fakeSqsService.Object, _fakeSqsConfig.Object);
            //act
            var actualResponse = await sqsQueue.SendMessageAsync(testMesssage, messageGroupId, deduplicationId);
            //assert
            Assert.NotNull(actualResponse.Item1);
            Assert.Equal(expectedResponse.MessageId, actualResponse.Item1);
            Assert.Equal((int)expectedResponse.HttpStatusCode, actualResponse.Item2);
        }

        [Fact]
        public async Task ReceiveMessageIsSuccessful()
        {
            //arrange
            var receiveMessageResponse = new ReceiveMessageResponse
            {
                Messages = new List<Message>
                {
                    new Message
                    {
                         Body = "This is a test message",
                         MessageId = "testmessageid",
                         ReceiptHandle = "testreceipthandle"
                    }
                }
            };
            _fakeSqsService.Setup(
                p => p.ReceiveMessageAsync(It.IsAny<ReceiveMessageRequest>(), default))
                .Returns(Task.FromResult(receiveMessageResponse));
            _fakeSqsConfig.Setup(c => c.Value).Returns(_fakeSqsConfigValues);
            var sqsQueue = new SqsMessageQueue(_fakeSqsService.Object, _fakeSqsConfig.Object);
            //act
            var response = await sqsQueue.ReceiveMessageAsync();
            //assert
            Assert.NotNull(response);
            Assert.Equal(receiveMessageResponse.Messages.Count, response.Count);
        }

        [Fact]
        public async Task DeleteMessageIsSuccessful()
        {
            var receiptHandle = "testreceipthandle";
            //arrange
            var deleteMessageResponse = _fixture.Create<DeleteMessageResponse>();
            _fakeSqsService.Setup(
                p => p.DeleteMessageAsync(It.IsAny<DeleteMessageRequest>(), default))
                .Returns(Task.FromResult(deleteMessageResponse));
            _fakeSqsConfig.Setup(c => c.Value).Returns(_fakeSqsConfigValues);
            var sqsQueue = new SqsMessageQueue(_fakeSqsService.Object, _fakeSqsConfig.Object);
            //act
            var response = await sqsQueue.DeleteMessageAsync(receiptHandle);
            //assert
            Assert.Equal((int)deleteMessageResponse.HttpStatusCode, response);
        }

        [Fact]
        public async Task GetRedrivePolicyIsSuccessful()
        {
            // arrange
            var queueUrl = "https://sqs.us-east-1.amazonaws.com/000000000000/fake-sqs-queue";
            var expectedRedrivePolicy = new JObject
            {
                ["deadLetterTargetArn"] = "arn:aws:sqs:us-east-1:000000000000:dead-letter-queue",
                ["maxReceiveCount"] = 5
            };

            var getQueueAttributesResponse = new GetQueueAttributesResponse
            {
                Attributes = new Dictionary<string, string>
                {
                    { "RedrivePolicy", expectedRedrivePolicy.ToString(Formatting.None) }
                }
            };

            _fakeSqsService.Setup(
                p => p.GetQueueAttributesAsync(It.Is<GetQueueAttributesRequest>(r => r.QueueUrl == queueUrl && r.AttributeNames.Contains("RedrivePolicy")), default))
                .Returns(Task.FromResult(getQueueAttributesResponse));
            _fakeSqsConfig.Setup(c => c.Value).Returns(_fakeSqsConfigValues);
            var sqsQueue = new SqsMessageQueue(_fakeSqsService.Object, _fakeSqsConfig.Object);

            // act
            var actualRedrivePolicy = await sqsQueue.GetRedrivePolicyAsync(queueUrl);

            // assert
            Assert.NotNull(actualRedrivePolicy);
            Assert.Equal(expectedRedrivePolicy.ToString(Formatting.None), actualRedrivePolicy?.ToString(Formatting.None));
        }

        [Fact]
        public async Task StartMessageMoveTaskIsSuccessful()
        {
            // arrange
            var sourceArn = "arn:aws:sqs:us-east-1:000000000000:source-queue";
            var destinationArn = "arn:aws:sqs:us-east-1:000000000000:destination-queue";
            var expectedResponse = new StartMessageMoveTaskResponse
            {
                HttpStatusCode = HttpStatusCode.OK
            };

            _fakeSqsService.Setup(
                p => p.StartMessageMoveTaskAsync(It.Is<StartMessageMoveTaskRequest>(r => r.SourceArn == sourceArn && r.DestinationArn == destinationArn), default))
                .Returns(Task.FromResult(expectedResponse));
            _fakeSqsConfig.Setup(c => c.Value).Returns(_fakeSqsConfigValues);
            var sqsQueue = new SqsMessageQueue(_fakeSqsService.Object, _fakeSqsConfig.Object);

            // act
            var responseStatusCode = await sqsQueue.StartMessageMoveTaskAsync(sourceArn, destinationArn);

            // assert
            Assert.Equal((int)expectedResponse.HttpStatusCode, responseStatusCode);
        }
    }
}
