namespace Amazon.Sqs.Wrapper.Interfaces
{
    /// <summary>
    /// Interface to provide generic message format when receiving messages from SQS
    /// </summary>
    public interface IMessage
    {
        public string Id { get; set; }
        public string Body { get; set; }
    }
}
