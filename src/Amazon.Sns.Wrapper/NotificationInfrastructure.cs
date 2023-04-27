using Amazon.SimpleNotificationService;
using Amazon.Sns.Wrapper.Implementations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amazon.Sns.Wrapper
{
    public class NotificationInfrastructure
    {
        public NotificationInfrastructure (IAmazonSimpleNotificationService client, ILogger<NotificationService> logger)
        {
            ServiceClient = client;
            Logger = logger;
        }

        public IAmazonSimpleNotificationService ServiceClient { get; private set; }
        public ILogger<NotificationService> Logger { get; private set; }
    }
}
