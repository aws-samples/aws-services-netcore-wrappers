using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.Sns.Wrapper.Interfaces;
using Amazon.Sns.Wrapper.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Amazon.Sns.Wrapper.Implementations
{
    public class MobilePushNotification : IMobilePushNotification
    {
        private readonly IAmazonSimpleNotificationService _client;
        internal MobilePushNotification(IAmazonSimpleNotificationService client)
        {
            _client = client;
        }
        
        public async Task<string> CreatePlatformApplicationAsync(string name, NotificationPlatform platform, Dictionary<string, string> attributes, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException($"Name cannot be null to create Platform Application");

            if (string.IsNullOrEmpty(platform.ToString()))
            {
                throw new ArgumentNullException($"Platform cannot be null to create Platform Application");
            }

            var request = new CreatePlatformApplicationRequest { Name = name, Platform = platform.ToString(), Attributes = attributes };
            var response = await _client.CreatePlatformApplicationAsync(request, cancellationToken);
            if (response == null || string.IsNullOrEmpty(response.PlatformApplicationArn))
            {
                throw new Exception($"Unable to Create Platform Application with name - {name}");
            }

            return response.PlatformApplicationArn;
        }

        public async Task<string> CreatePlatformEndpointAsync(string platformApplicationArn, string token, string description, Dictionary<string, string> attributes, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(platformApplicationArn))
                throw new ArgumentNullException($"Platform Application Arn cannot be null to create Platform Endpoint");

            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException($"Token cannot be null to create Platform Endpoint");

            var request = new CreatePlatformEndpointRequest { PlatformApplicationArn = platformApplicationArn, Token = token, CustomUserData = description, Attributes = attributes };
            var response = await _client.CreatePlatformEndpointAsync(request, cancellationToken);
            if (response == null || string.IsNullOrEmpty(response.EndpointArn))
            {
                throw new Exception($"Unable to Create Platform Endpoint for Platform Application: {platformApplicationArn}");
            }

            return response.EndpointArn;
        }

        public async Task<bool> DeleteEndpointAsync(string endpointArn, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(endpointArn))
                throw new ArgumentNullException($"Endpoint Arn cannot be null to delete Platform Endpoint");

            var request = new DeleteEndpointRequest { EndpointArn = endpointArn };
            var response = await _client.DeleteEndpointAsync(request, cancellationToken);
            if (response == null || response.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"Unable to Delete Platform Endpoint with Arn- {endpointArn}");
            }

            return true;
        }

        public async Task<bool> DeletePlatformApplicationAsync(string platformApplicationArn, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(platformApplicationArn))
                throw new ArgumentNullException($"Platform Application Arn cannot be null to delete Platform Application");

            var request = new DeletePlatformApplicationRequest { PlatformApplicationArn = platformApplicationArn };
            var response = await _client.DeletePlatformApplicationAsync(request, cancellationToken);
            if (response == null || response.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"Unable to Delete Platform Application with Arn- {platformApplicationArn}");
            }

            return true;
        }

        public async Task<Dictionary<string, string>> GetEndpointAttributesAsync(string endpointArn, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(endpointArn))
                throw new ArgumentNullException($"Endpoint Arn cannot be null to get Platform Endpoint attributes");

            var request = new GetEndpointAttributesRequest { EndpointArn = endpointArn };
            var response = await _client.GetEndpointAttributesAsync(request, cancellationToken);
            if (response == null || response.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"Unable to get Endpoint attributes with Arn- {endpointArn}");
            }

            return response.Attributes;
        }

        public async Task<Dictionary<string, string>> GetPlatformApplicationAttributesAsync(string platformApplicationArn, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(platformApplicationArn))
                throw new ArgumentNullException($"Platform Application Arn cannot be null to Get Platform Application Attributes");

            var request = new GetPlatformApplicationAttributesRequest { PlatformApplicationArn = platformApplicationArn };
            var response = await _client.GetPlatformApplicationAttributesAsync(request, cancellationToken);
            if (response == null || response.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"Unable to get Platform Application Attributes with Application Arn- {platformApplicationArn}");
            }

            return response.Attributes;
        }

        public async Task<List<Endpoint>> ListEndpointsByPlatformApplicationAsync(string platformApplicationArn, int maxRecords = 100, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(platformApplicationArn))
                throw new ArgumentNullException($"Platform Application Arn cannot be null to Get Endpoints");

            var endpoints = new List<Endpoint>();
            int recordsToGet = maxRecords;

            var request = new ListEndpointsByPlatformApplicationRequest { PlatformApplicationArn = platformApplicationArn };
            var response = await _client.ListEndpointsByPlatformApplicationAsync(request, cancellationToken);
            if (response == null || response.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"Unable to get Platform Application Endpoints");
            }

            if (maxRecords <= 100)
            {
                return response.Endpoints;
            }

            endpoints.AddRange(response.Endpoints);
            recordsToGet -= 100;
            while (recordsToGet > 0)
            {
                request = new ListEndpointsByPlatformApplicationRequest { PlatformApplicationArn = platformApplicationArn, NextToken = response.NextToken };
                response = await _client.ListEndpointsByPlatformApplicationAsync(request, cancellationToken);
                if (response == null || response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception($"Unable to get Platform Applications");
                }

                endpoints.AddRange(response.Endpoints);
                recordsToGet -= 100;
            }

            return endpoints;
        }

        public async Task<List<PlatformApplication>> ListPlatformApplicationsAsync(int maxRecords = 100, CancellationToken cancellationToken = default)
        {
            var applications = new List<PlatformApplication>();
            int recordsToGet = maxRecords;

            var request = new ListPlatformApplicationsRequest();
            var response = await _client.ListPlatformApplicationsAsync(request, cancellationToken);
            if (response == null || response.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"Unable to get Platform Applications");
            }

            if (maxRecords <= 100)
            {
                return response.PlatformApplications;
            }

            applications.AddRange(response.PlatformApplications);
            recordsToGet -= 100;
            while (recordsToGet > 0)
            {
                request = new ListPlatformApplicationsRequest { NextToken = response.NextToken };
                response = await _client.ListPlatformApplicationsAsync(request, cancellationToken);
                if (response == null || response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception($"Unable to get Platform Applications");
                }

                applications.AddRange(response.PlatformApplications);
                recordsToGet -= 100;
            }

            return applications;
        }

        public async Task<bool> SetEndpointAttributesAsync(string endpointArn, Dictionary<string, string> attributes, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(endpointArn))
                throw new ArgumentNullException($"Endpoint Arn cannot be null to set Endpoint attributes");

            var request = new SetEndpointAttributesRequest { EndpointArn = endpointArn, Attributes = attributes };
            var response = await _client.SetEndpointAttributesAsync(request, cancellationToken);
            if (response == null || response.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"Unable to set Endpoint attributes with Arn- {endpointArn}");
            }

            return true;
        }

        public async Task<bool> SetPlatformApplicationAttributesAsync(string platformApplicationArn, Dictionary<string, string> attributes, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(platformApplicationArn))
                throw new ArgumentNullException($"Platform Application Arn cannot be null to set Platform Application Attributes");

            var request = new SetPlatformApplicationAttributesRequest { PlatformApplicationArn = platformApplicationArn, Attributes = attributes };
            var response = await _client.SetPlatformApplicationAttributesAsync(request, cancellationToken);
            if (response == null || response.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"Unable to set Platform Application Attributes with Application Arn- {platformApplicationArn}");
            }

            return true;
        }
    }
}
