using Amazon.S3.Wrapper.Enums;
using Amazon.S3.Wrapper.Utils;

namespace Amazon.S3.Wrapper.Extensions
{
    public static class ContentTypeExtension
    {
        public static string Value(this ContentType contentType)
        {
            switch (contentType)
            {
                case ContentType.Csv:
                    return Constants.Csv;
                case ContentType.Text:
                    return Constants.Text;
                case ContentType.Json:
                    return Constants.Json;
                default:
                    return null;
            }
        }
    }
}
