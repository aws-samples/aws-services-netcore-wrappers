using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using AutoFixture;
using Amazon.Sns.Wrapper.Implementations;
using Amazon.Sns.Wrapper.Interfaces;
using Moq;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Amazon.Sns.Wrapper.Tests
{
    public class NotificationSubscriptionTests
    {
        private Mock<IAmazonSimpleNotificationService> _fakeSnsClient;
        private Fixture _fixture;
        private INotificationSubscription _subscription = null;

        public NotificationSubscriptionTests()
        {
            _fixture = new Fixture();
            _fakeSnsClient = new Mock<IAmazonSimpleNotificationService>();
        }

        [Fact]
        public async Task UnSubscribeIsSuccessful()
        {
            //Setup Fake Response for AddPermissionAsync method 
            var fakeResponse = GetResponseFixture<UnsubscribeResponse>();
            _fakeSnsClient.Setup(p => p.UnsubscribeAsync(It.IsAny<string>(), default))
                          .Returns(Task.FromResult(fakeResponse));

            var response = await Subscription.UnsubscribeAsync();
            Assert.True(response);
        }

        #region Attributes

        [Fact]
        public async Task AddAttributesIsSuccessful()
        {
            string fakeAttributeName = "fakeAttribute";
            string fakeAttribute = "test";

            //Setup Fake Response for AddPermissionAsync method 
            var fakeResponse = GetResponseFixture<SetSubscriptionAttributesResponse>();
            _fakeSnsClient.Setup(p => p.SetSubscriptionAttributesAsync(It.IsAny<SetSubscriptionAttributesRequest>(), default))
                          .Returns(Task.FromResult(fakeResponse));

            var response = await Subscription.AddAtrributeAsync(fakeAttributeName, fakeAttribute);
            Assert.True(response);
        }

        [Fact]
        public async Task GetAttributesIsSuccessful()
        {
            //Setup Fake Response for AddPermissionAsync method 
            var fakeResponse = GetResponseFixture<GetSubscriptionAttributesResponse>();
            _fakeSnsClient.Setup(p => p.GetSubscriptionAttributesAsync(It.IsAny<GetSubscriptionAttributesRequest>(), default))
                          .Returns(Task.FromResult(fakeResponse));

            var response = await Subscription.GetAttributesAsync();
            Assert.Equal(response.Count, fakeResponse.Attributes.Count);
        }

        #endregion
        #region Private methods
        private INotificationSubscription Subscription
        {
            get
            {
                return GetNotificationSubscriptionAsync().Result;
            }
        }
        private async Task<INotificationSubscription> GetNotificationSubscriptionAsync()
        {
            if (_subscription != null)
                return _subscription;

            //var fakeSnsClient = new Mock<IAmazonSimpleNotificationService>();
            string topicName = "fake-mytopic";
            var getTopicResponse = _fixture.Create<Topic>();
            _fakeSnsClient.Setup(p => p.FindTopicAsync(It.IsAny<string>()))
                          .Returns(Task.FromResult(getTopicResponse));

            var service = new NotificationService(_fakeSnsClient.Object);
            var topic = await service.GetTopicAsync(topicName);

            var fakeResponse = GetResponseFixture<ListSubscriptionsByTopicResponse>();
            _fakeSnsClient.Setup(p => p.ListSubscriptionsByTopicAsync(It.IsAny<string>(), default))
                          .Returns(Task.FromResult(fakeResponse));

            var subscriptions = await topic.ListSubscriptionsAsync();
            _subscription = subscriptions.FirstOrDefault();
            return _subscription;
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
