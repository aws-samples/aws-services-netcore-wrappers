using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Wrapper.Enums;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Amazon.S3.Wrapper
{
    /// <summary>
    /// Provides a simple implementation to upload files to Amazon Simple Storage Service (Amazon S3)
    /// </summary>
    public interface IS3Client
    {
        /// <summary>
        /// Upload an object to S3 bucket
        /// </summary>
        /// <param name="key">This key is used to identify the object in S3</param>
        /// <param name="content">content to be uploaded</param>
        /// <returns>PutObjectResponse object</returns>
        Task<bool> UploadObjectAsync(string key, Stream content, CancellationToken cancellationToken = default);

        /// <summary>
        /// Upload an object to S3 bucket
        /// </summary>
        /// <param name="key">This key is used to identify the object in S3</param>
        /// <param name="content">content to be uploaded</param>
        /// <returns>PutObjectResponse object</returns>
        Task<PutObjectResponse> PutObjectAsync(string key, string content, CancellationToken cancellationToken = default);

        /// <summary>
        /// Upload an object to S3 bucket
        /// </summary>
        /// <param name="key">This key is used to identify the object in S3</param>
        /// <param name="content">content to be uploaded</param>
        /// <param name="contentType">Overrides contentype provided in registry</param>
        /// <param name="storageClass">Overrides storageClass provided in registry<param>
        /// <returns>PutObjectResponse object</returns>
        Task<PutObjectResponse> PutObjectAsync(string key, string content, ContentType? contentType = null, S3StorageClass storageClass = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get an object from S3 bucket
        /// </summary>
        /// <param name="key">This key is used to identify the object in S3</param>
        /// <returns>GetObjectResponse object</returns>
        Task<GetObjectResponse> GetObjectAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete an object from bucket. 
        /// </summary>
        /// <param name="key">This key is used to identify the object in S3</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>GetObjectResponse</returns>
        Task<DeleteObjectResponse> DeleteObjectAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete list of objects from bucket. 
        /// </summary>
        /// <param name="keys">List of object keys to delete.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>DeleteObjectsResponse</returns>
        Task<DeleteObjectsResponse> DeleteObjectsAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default);

        /// <summary>
        /// Check if the file exists or not. 
        /// </summary>
        /// <param name="fileName">List of object keys to delete.</param>
        /// <param name="versionId">Propagates notification that operations should be canceled.</param>
        /// <returns>DeleteObjectsResponse</returns>
        Task<bool> IsFileExistsAsync(string fileName, string versionId);
    }
}
