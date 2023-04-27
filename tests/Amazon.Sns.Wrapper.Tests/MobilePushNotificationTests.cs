using Amazon.Runtime;
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
    public class MobilePushNotificationTests
    {
        private Mock<IAmazonSimpleNotificationService> _fakeSnsClient;
        private Fixture _fixture;

        public MobilePushNotificationTests()
        {
            _fixture = new Fixture();
            _fakeSnsClient = new Mock<IAmazonSimpleNotificationService>();
        }

        [Fact]
        public async Task CreatePlatformApplicationIsSuccessful()
        {
            var fakeRequest = _fixture.Create<CreatePlatformApplicationRequest>();
            var fakeResponse = GetResponseFixture<CreatePlatformApplicationResponse>();
            _fakeSnsClient.Setup(p => p.CreatePlatformApplicationAsync(It.IsAny<CreatePlatformApplicationRequest>(), default))
                          .Returns(Task.FromResult(fakeResponse));

            var response = await GetNotificationService().PushNotification.CreatePlatformApplicationAsync(fakeRequest.Name, Models.NotificationPlatform.GCM, fakeRequest.Attributes);
            Assert.Equal(response, fakeResponse.PlatformApplicationArn);
        }

        [Fact]
        public async Task CreatePlatformEndpointIsSuccessful()
        {
            var fakeRequest = _fixture.Create<CreatePlatformEndpointRequest>();
            var fakeResponse = GetResponseFixture<CreatePlatformEndpointResponse>();
            _fakeSnsClient.Setup(p => p.CreatePlatformEndpointAsync(It.IsAny<CreatePlatformEndpointRequest>(), default))
                          .Returns(Task.FromResult(fakeResponse));

            var response = await GetNotificationService().PushNotification.CreatePlatformEndpointAsync(fakeRequest.PlatformApplicationArn, fakeRequest.Token, fakeRequest.CustomUserData, fakeRequest.Attributes);
            Assert.Equal(response, fakeResponse.EndpointArn);
        }

        [Fact]
        public async Task DeletePlatformApplicationIsSuccessful()
        {
            var fakeRequest = _fixture.Create<DeletePlatformApplicationRequest>();
            var fakeResponse = GetResponseFixture<DeletePlatformApplicationResponse>();
            _fakeSnsClient.Setup(p => p.DeletePlatformApplicationAsync(It.IsAny<DeletePlatformApplicationRequest>(), default))
                          .Returns(Task.FromResult(fakeResponse));

            var response = await GetNotificationService().PushNotification.DeletePlatformApplicationAsync(fakeRequest.PlatformApplicationArn);
            Assert.True(response);
        }

        [Fact]
        public async Task DeletePlatformEndpointIsSuccessful()
        {
            var fakeRequest = _fixture.Create<DeleteEndpointRequest>();
            var fakeResponse = GetResponseFixture<DeleteEndpointResponse>();
            _fakeSnsClient.Setup(p => p.DeleteEndpointAsync(It.IsAny<DeleteEndpointRequest>(), default))
                          .Returns(Task.FromResult(fakeResponse));

            var response = await GetNotificationService().PushNotification.DeleteEndpointAsync(fakeRequest.EndpointArn);
            Assert.True(response);
        }

        [Fact]
        public async Task GetPlatformApplicationAttributesIsSuccessful()
        {
            var fakeRequest = _fixture.Create<GetPlatformApplicationAttributesRequest>();
            var fakeResponse = GetResponseFixture<GetPlatformApplicationAttributesResponse>();
            _fakeSnsClient.Setup(p => p.GetPlatformApplicationAttributesAsync(It.IsAny<GetPlatformApplicationAttributesRequest>(), default))
                          .Returns(Task.FromResult(fakeResponse));

            var response = await GetNotificationService().PushNotification.GetPlatformApplicationAttributesAsync(fakeRequest.PlatformApplicationArn);
            Assert.Equal(response.Count, fakeResponse.Attributes.Count);
        }

        [Fact]
        public async Task GetPlatformEndpointAttributesIsSuccessful()
        {
            var fakeRequest = _fixture.Create<GetEndpointAttributesRequest>();
            var fakeResponse = GetResponseFixture<GetEndpointAttributesResponse>();
            _fakeSnsClient.Setup(p => p.GetEndpointAttributesAsync(It.IsAny<GetEndpointAttributesRequest>(), default))
                          .Returns(Task.FromResult(fakeResponse));

            var response = await GetNotificationService().PushNotification.GetEndpointAttributesAsync(fakeRequest.EndpointArn);
            Assert.Equal(response.Count, fakeResponse.Attributes.Count);
        }


        [Fact]
        public async Task SetPlatformApplicationAttributesIsSuccessful()
        {
            var fakeRequest = _fixture.Create<SetPlatformApplicationAttributesRequest>();
            var fakeResponse = GetResponseFixture<SetPlatformApplicationAttributesResponse>();
            _fakeSnsClient.Setup(p => p.SetPlatformApplicationAttributesAsync(It.IsAny<SetPlatformApplicationAttributesRequest>(), default))
                          .Returns(Task.FromResult(fakeResponse));

            var response = await GetNotificationService().PushNotification.SetPlatformApplicationAttributesAsync(fakeRequest.PlatformApplicationArn, fakeRequest.Attributes);
            Assert.True(response);
        }

        [Fact]
        public async Task SetPlatformEndpointAttributesIsSuccessful()
        {
            var fakeRequest = _fixture.Create<SetEndpointAttributesRequest>();
            var fakeResponse = GetResponseFixture<SetEndpointAttributesResponse>();
            _fakeSnsClient.Setup(p => p.SetEndpointAttributesAsync(It.IsAny<SetEndpointAttributesRequest>(), default))
                          .Returns(Task.FromResult(fakeResponse));

            var response = await GetNotificationService().PushNotification.SetEndpointAttributesAsync(fakeRequest.EndpointArn, fakeRequest.Attributes);
            Assert.True(response);
        }
        [Fact]
        public async Task ListPlatformApplicationsIsSuccessful()
        {
            var fakeRequest = _fixture.Create<ListPlatformApplicationsRequest>();
            var fakeResponse = GetResponseFixture<ListPlatformApplicationsResponse>();
            _fakeSnsClient.Setup(p => p.ListPlatformApplicationsAsync(It.IsAny<ListPlatformApplicationsRequest>(), default))
                          .Returns(Task.FromResult(fakeResponse));

            var response = await GetNotificationService().PushNotification.ListPlatformApplicationsAsync();
            Assert.Equal(response.Count, fakeResponse.PlatformApplications.Count);
        }

        [Fact]
        public async Task ListPlatformEndpointsIsSuccessful()
        {
            var fakeRequest = _fixture.Create<ListEndpointsByPlatformApplicationRequest>();
            var fakeResponse = GetResponseFixture<ListEndpointsByPlatformApplicationResponse>();
            _fakeSnsClient.Setup(p => p.ListEndpointsByPlatformApplicationAsync(It.IsAny<ListEndpointsByPlatformApplicationRequest>(), default))
                          .Returns(Task.FromResult(fakeResponse));

            var response = await GetNotificationService().PushNotification.ListEndpointsByPlatformApplicationAsync(fakeRequest.PlatformApplicationArn);
            Assert.Equal(response.Count, fakeResponse.Endpoints.Count);
        }


        private INotificationService GetNotificationService()
        {
            return new NotificationService(_fakeSnsClient.Object);
        }
        private T GetResponseFixture<T>() where T : AmazonWebServiceResponse
        {
            var response = _fixture.Create<T>();
            response.HttpStatusCode = System.Net.HttpStatusCode.OK;
            return response;
        }
    }
}
