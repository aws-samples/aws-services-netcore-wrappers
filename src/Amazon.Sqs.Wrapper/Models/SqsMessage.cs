using Amazon.Sqs.Wrapper.Interfaces;

namespace Amazon.Sqs.Wrapper
{
    /// <summary>
    /// class providing sqs message format
    /// </summary>
    public class SqsMessage : IMessage
    {
        public string Id { get ; set ; } //Receipt Handle for SQS Message as its required for message deletion
        public string Body { get ; set ; }
    }
}
