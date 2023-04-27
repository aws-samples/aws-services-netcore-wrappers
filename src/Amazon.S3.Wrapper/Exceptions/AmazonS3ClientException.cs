using System;

namespace Amazon.S3.Wrapper.Exceptions
{
    public class AmazonS3ClientException : Exception
    {
        public AmazonS3ClientException()
        {
        }

        public AmazonS3ClientException(string message) : base(message)
        {
        }

        public AmazonS3ClientException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
