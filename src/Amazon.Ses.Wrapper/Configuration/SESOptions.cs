namespace Amazon.Ses.Wrapper
{
    public class SESOptions
    {
        public string? Region { get; set; }
        public bool AssumeRoleFlag { get; set; }
        public string? AssumeRoleArn { get; set; }
        public string? AssumeRoleSessionName { get; set; }
        public string? Sender { get; set; }
    }
}