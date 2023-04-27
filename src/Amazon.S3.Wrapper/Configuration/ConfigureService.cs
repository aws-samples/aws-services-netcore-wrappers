using Amazon.S3;
using Amazon.S3.Wrapper;
using Amazon.S3.Wrapper.Enums;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Configuration for S3 client
    /// </summary>
    public static class ConfigureService
    {
        /// <summary>
        /// Register a new instance of S3Client
        /// </summary>
        /// <param name="IServiceCollection">Collection of service descriptors</param>
        /// <param name="amazonS3Config">Configuration for AmazonS3 service</param>
        /// <param name="bucketName">Bucket name</param>
        /// <param name="contentType">File content type. Default value "json"</param>
        /// <param name="storageClass">S3 File storage class. Default value S3StorageClass.Standard</param>
        public static IServiceCollection RegisterIAmazonS3Client(
         this IServiceCollection services,         
         AmazonS3Config amazonS3Config,
         string bucketName,
         ContentType? contentType = null,
         S3StorageClass storageClass = null)
        {
            services.AddSingleton<IAmazonS3>(_ => new AmazonS3Client(amazonS3Config));

            services.AddSingleton<IS3Client>(provider =>
            {
                var s3Client = provider.GetService<IAmazonS3>();
                return new S3Client(s3Client, bucketName, contentType, storageClass);
            });
            return services;
        }        
    }
}
