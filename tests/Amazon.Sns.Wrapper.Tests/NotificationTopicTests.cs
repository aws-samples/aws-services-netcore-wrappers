using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using AutoFixture;
using Amazon.Sns.Wrapper.Implementations;
using Amazon.Sns.Wrapper.Interfaces;
using Amazon.Sns.Wrapper.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Amazon.Sns.Wrapper.Tests
{
    public class NotificationTopicTests
    {
        private Mock<IAmazonSimpleNotificationService> _fakeSnsClient;
        private Fixture _fixture;
        private INotificationTopic _topic = null;

        public NotificationTopicTests()
        {
            _fixture = new Fixture();
            _fakeSnsClient = new Mock<IAmazonSimpleNotificationService>();
        }

        [Fact]
        public async Task AddPermissionsIsSuccessful()
        {
            string fakeLabel = "fakeLabel";
            var fakeAccountIds = new List<string>{ "123456789" };
            var fakeActionNames = new List<string>{ "Publish" };

            //Setup Fake Response for AddPermissionAsync method 
            _fakeSnsClient.Setup(p => p.AddPermissionAsync(It.IsAny<AddPermissionRequest>(), default))
                          .Returns(Task.FromResult(GetResponseFixture<AddPermissionResponse>()));

            var response = await Topic.AddPermissionAsync(fakeLabel, fakeAccountIds, fakeActionNames);
            Assert.True(response);
        }

        [Fact]
        public async Task RemovePermissionsIsSuccessful()
        {
            string fakeLabel = "fakeLabel";

            //Setup Fake Response for AddPermissionAsync method 
            _fakeSnsClient.Setup(p => p.RemovePermissionAsync(It.IsAny<RemovePermissionRequest>(), default))
                          .Returns(Task.FromResult(GetResponseFixture<RemovePermissionResponse>()));

            var response = await Topic.RemovePermissionAsync(fakeLabel);
            Assert.True(response);
        }

        [Fact]
        public async Task DeleteIsSuccessful()
        {
            //Setup Fake Response for AddPermissionAsync method 
            _fakeSnsClient.Setup(p => p.DeleteTopicAsync(It.IsAny<DeleteTopicRequest>(), default))
                          .Returns(Task.FromResult(GetResponseFixture<DeleteTopicResponse>()));

            var response = await Topic.DeleteAsync();
            Assert.True(response);
        }

        [Fact]
        public async Task PublishIsSuccessful()
        {
            var fakeMessage = "this is a test message";
            //Setup Fake Response for AddPermissionAsync method 
            var fakeResponse = GetResponseFixture<PublishResponse>();
            _fakeSnsClient.Setup(p => p.PublishAsync(It.IsAny<PublishRequest>(), default))
                          .Returns(Task.FromResult(fakeResponse));

            var response = await Topic.PublishAsync(fakeMessage);
            Assert.Equal(response, fakeResponse.MessageId);
        }

        #region Subscriptions
        [Fact]
        public async Task SubscribeIsSuccessful()
        {
            var fakeProtocol = NotificationProtocol.http;
            string fakeEndpoint = "http://test.com/test";

            //Setup Fake Response for AddPermissionAsync method 
            var fakeResponse = GetResponseFixture<SubscribeResponse>();
            _fakeSnsClient.Setup(p => p.SubscribeAsync(It.IsAny<SubscribeRequest>(), default))
                          .Returns(Task.FromResult(fakeResponse));

            var response = await Topic.SubscribeAsync(fakeProtocol, fakeEndpoint);
            Assert.Equal(response.Arn, fakeResponse.SubscriptionArn);
        }

        [Fact]
        public async Task ConfirmSubscriptionIsSuccessful()
        {
            var fakeToken = "abcd-2345-123def"; ;

            //Setup Fake Response for AddPermissionAsync method 
            var fakeResponse = GetResponseFixture<ConfirmSubscriptionResponse>();
            _fakeSnsClient.Setup(p => p.ConfirmSubscriptionAsync(It.IsAny<ConfirmSubscriptionRequest>(), default))
                          .Returns(Task.FromResult(fakeResponse));

            var response = await Topic.ConfirmSubscriptionAsync(fakeToken);
            Assert.Equal(response.Arn, fakeResponse.SubscriptionArn);
        }

        [Fact]
        public async Task ListSubscriptionsIsSuccessful()
        {
            //Setup Fake Response for AddPermissionAsync method 
            var fakeResponse = GetResponseFixture<ListSubscriptionsByTopicResponse>();
            _fakeSnsClient.Setup(p => p.ListSubscriptionsByTopicAsync(It.IsAny<string>(), default))
                          .Returns(Task.FromResult(fakeResponse));

            var response = await Topic.ListSubscriptionsAsync();
            Assert.Equal(response.Count, fakeResponse.Subscriptions.Count);
        }
        #endregion

        #region Attributes

        [Fact]
        public async Task AddAttributesIsSuccessful()
        {
            string fakeAttributeName = "fakeAttribute";
            string fakeAttribute = "test";

            //Setup Fake Response for AddPermissionAsync method 
            var fakeResponse = GetResponseFixture<SetTopicAttributesResponse>();
            _fakeSnsClient.Setup(p => p.SetTopicAttributesAsync(It.IsAny<SetTopicAttributesRequest>(), default))
                          .Returns(Task.FromResult(fakeResponse));

            var response = await Topic.AddAttributeAsync(fakeAttributeName, fakeAttribute);
            Assert.True(response);
        }
      
        [Fact]
        public async Task GetAttributesIsSuccessful()
        {
            //Setup Fake Response for AddPermissionAsync method 
            var fakeResponse = GetResponseFixture<GetTopicAttributesResponse>();
            _fakeSnsClient.Setup(p => p.GetTopicAttributesAsync(It.IsAny<GetTopicAttributesRequest>(), default))
                          .Returns(Task.FromResult(fakeResponse));

            var response = await Topic.GetAttributesAsync();
            Assert.Equal(response.Count, fakeResponse.Attributes.Count);
        }

        #endregion

        #region Private methods
        private INotificationTopic Topic
        {
            get
            {
                return GetNotificationTopicAsync().Result;
            }
        }
        private async Task<INotificationTopic> GetNotificationTopicAsync()
        {
            if (_topic != null)
                return _topic;

            //var fakeSnsClient = new Mock<IAmazonSimpleNotificationService>();
            string topicName = "fake-mytopic";
            var getTopicResponse = _fixture.Create<Topic>();
            _fakeSnsClient.Setup(p => p.FindTopicAsync(It.IsAny<string>()))
                          .Returns(Task.FromResult(getTopicResponse));

            var service = new NotificationService(_fakeSnsClient.Object);
            _topic = await service.GetTopicAsync(topicName);
            return _topic;
        }
        private T GetResponseFixture<T>() where T : AmazonWebServiceResponse
        {
            var response = _fixture.Create<T>();
            response.HttpStatusCode = System.Net.HttpStatusCode.OK;
            return response;
        }
        #endregion
    }
}
