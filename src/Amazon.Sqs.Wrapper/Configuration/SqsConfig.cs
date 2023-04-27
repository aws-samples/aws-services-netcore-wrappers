namespace Amazon.Sqs.Wrapper.Configuration
{
    /// <summary>
    /// SQS configuration
    /// </summary>
    public class SqsConfig
    {
        public string QueueName { get; set; }
        public string QueueUrl { get; set; }
        public string Region { get; set; }
        public int MaxNumberOfMessages { get; set; }
        public int WaitTimeSeconds { get; set; }
    }
}
