using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using AutoFixture;
using Amazon.Sns.Wrapper.Implementations;
using Amazon.Sns.Wrapper.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Amazon.Sns.Wrapper.Tests
{
    public class NotificationServiceTests
    {
        private Mock<IAmazonSimpleNotificationService> _fakeSnsClient;
        private Fixture _fixture;

        public NotificationServiceTests()
        {
            _fixture = new Fixture();
            _fakeSnsClient = new Mock<IAmazonSimpleNotificationService>();
        }

        [Fact]
        public async Task CreateTopicIsSuccessful()
        {
            string topicName = "fake-mytopic";
            var createTopicResponse = _fixture.Create<CreateTopicResponse>();
            _fakeSnsClient.Setup(p => p.CreateTopicAsync(It.IsAny<CreateTopicRequest>(), default))
                          .Returns(Task.FromResult(createTopicResponse));

            var response = await GetNotificationService().CreateTopicAsync(topicName);
            Assert.Equal(response.Arn, createTopicResponse.TopicArn);
        }

        [Fact]
        public async Task GetTopicIsSuccessful()
        {
            string topicName = "fake-mytopic";
            var getTopicResponse = _fixture.Create<Topic>();
            _fakeSnsClient.Setup(p => p.FindTopicAsync(It.IsAny<string>()))
                          .Returns(Task.FromResult(getTopicResponse));

            var response = await GetNotificationService().GetTopicAsync(topicName);
            Assert.Equal(response.Arn, getTopicResponse.TopicArn);
        }
        private INotificationService GetNotificationService()
        {
            return new NotificationService(_fakeSnsClient.Object);
        }
    }
}
