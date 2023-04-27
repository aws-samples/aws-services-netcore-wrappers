using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Wrapper.Enums;
using Amazon.S3.Wrapper.Exceptions;
using Amazon.S3.Wrapper.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Amazon.S3.Wrapper
{
    public class S3Client : IS3Client
    {
        private readonly S3StorageClass _storageClass;
        private readonly ContentType _contentType;
        private readonly string _bucketName;
        private readonly IAmazonS3 _client;

        /// <summary>
        /// Creates a new instance of amazon s3 client
        /// </summary>
        /// <param name="amazonS3">Client for accessing S3</param>
        /// <param name="bucketName">Bucket name</param>
        /// <param name="contentType">Default value xml</param>
        /// <param name="storageClass">Default value S3StorageClass.Standard</param>
        public S3Client(IAmazonS3 amazonS3, string bucketName, ContentType? contentType, S3StorageClass storageClass)
        {
            if (string.IsNullOrWhiteSpace(bucketName))
            {
                throw new ArgumentNullException(nameof(bucketName));
            }

            _client = amazonS3;
            _bucketName = bucketName;
            _contentType = contentType ?? ContentType.Json;
            _storageClass = storageClass ?? S3StorageClass.Standard;
        }

        public S3Client(string region, string bucketName, ContentType? contentType, S3StorageClass storageClass)
        {
            if (string.IsNullOrWhiteSpace(region))
            {
                throw new ArgumentNullException(nameof(region));
            }

            if (string.IsNullOrWhiteSpace(bucketName))
            {
                throw new ArgumentNullException(nameof(bucketName));
            }

            var s3Config = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(region),
                MaxErrorRetry = 1,
            };

            _client = new AmazonS3Client(s3Config);
            _bucketName = bucketName;
            _contentType = contentType ?? ContentType.Json;
            _storageClass = storageClass ?? S3StorageClass.Standard;
        }

        /// <summary>
        /// Upload a object as multipart upload to s3 bucket defined in initialization
        /// </summary>
        /// <param name="key">This key is used to identify the object in S3</param>
        /// <param name="content">content to be uploaded</param>
        /// <returns>PutObjectResponse object</returns>
        public async Task<bool> UploadObjectAsync(string key, Stream content, CancellationToken cancellationToken = default)
        {
            var result = await UploadObjectAsync(key, content, _contentType, _storageClass, cancellationToken).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Upload a object to s3 bucket using Multipart upload
        /// </summary>
        /// <param name="key">This key is used to identify the object in S3</param>
        /// <param name="content">content to be uploaded</param>
        /// <param name="contentType">Overrides contentype provided in registry</param>
        /// <param name="storageClass">Overrides storageClass provided in registry<param>
        /// <returns></returns>
        public async Task<bool> UploadObjectAsync(string key, Stream content, ContentType? contentType = null, S3StorageClass storageClass = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            var requestStorageClass = storageClass ?? _storageClass;
            var requestContentType = contentType ?? _contentType;

            var transferUtility =
                  new TransferUtility(_client);
            try
            {
                var request = new TransferUtilityUploadRequest
                {
                    BucketName = _bucketName,
                    Key = key,
                    StorageClass = requestStorageClass,
                    ContentType = requestContentType.Value(),
                    InputStream = content
                };
                await transferUtility.UploadAsync(request, cancellationToken);
                return true;
            }
            catch (Exception e)
            {
                var errorMessage = $"Error encountered {e.Message}.{Environment.NewLine}" +
                    $"It was not possible to upload the object.{Environment.NewLine}" +
                    $"BucketName: {_bucketName}{Environment.NewLine}" +
                    $"Key: {key}{Environment.NewLine}" +
                    $"ContentBody: {content}{Environment.NewLine}";

                //TODO: Try to abort the upload using upload id
                await transferUtility.AbortMultipartUploadsAsync(_bucketName, DateTime.Now);
                throw new AmazonS3ClientException(errorMessage, e);
            }
        }

        /// <summary>
        /// Upload a object to s3 bucket defined in initialization
        /// </summary>
        /// <param name="key">This key is used to identify the object in S3</param>
        /// <param name="content">content to be uploaded</param>
        /// <returns>PutObjectResponse object</returns>
        public async Task<PutObjectResponse> PutObjectAsync(string key, string content, CancellationToken cancellationToken = default)
        {
            var result = await PutObjectAsync(key, content, _contentType, _storageClass, cancellationToken).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Upload a object to s3 bucket
        /// </summary>
        /// <param name="key">This key is used to identify the object in S3</param>
        /// <param name="content">content to be uploaded</param>
        /// <param name="contentType">Overrides contentype provided in registry</param>
        /// <param name="storageClass">Overrides storageClass provided in registry<param>
        /// <returns></returns>
        public async Task<PutObjectResponse> PutObjectAsync(string key, string content, ContentType? contentType = null, S3StorageClass storageClass = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentNullException(nameof(content));
            }

            var requestStorageClass = storageClass ?? _storageClass;
            var requestContentType = contentType ?? _contentType;

            try
            {
                var putRequest = new PutObjectRequest
                {
                    BucketName = _bucketName,
                    Key = key,
                    StorageClass = requestStorageClass,
                    ContentType = requestContentType.Value(),
                    ContentBody = content
                };

                var result = await _client.PutObjectAsync(putRequest, cancellationToken).ConfigureAwait(false);

                return result;
            }
            catch (Exception e)
            {
                var errorMessage = $"Error encountered {e.Message}.{Environment.NewLine}" +
                    $"It was not possible to upload the object.{Environment.NewLine}" +
                    $"BucketName: {_bucketName}{Environment.NewLine}" +
                    $"Key: {key}{Environment.NewLine}" +
                    $"StorageClass: {requestStorageClass}{Environment.NewLine}" +
                    $"ContentType: {requestContentType}{Environment.NewLine}" +
                    $"ContentBody: {content}{Environment.NewLine}";
                throw new AmazonS3ClientException(errorMessage, e);
            }
        }

        /// <summary>
        /// Get object from s3 bucket
        /// </summary>
        /// <param name="key">This key is used to identify the object in S3</param>
        /// <returns></returns>
        public async Task<GetObjectResponse> GetObjectAsync(string key, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            try
            {
                var request = new GetObjectRequest
                {
                    BucketName = _bucketName,
                    Key = key
                };

                var result = await _client.GetObjectAsync(request, cancellationToken).ConfigureAwait(false);

                return result;
            }
            catch (Exception e)
            {
                var errorMessage = $"Error encountered {e.Message}.{Environment.NewLine}" +
                    $"It was not possible to get the object.{Environment.NewLine}" +
                    $"BucketName: {_bucketName}{Environment.NewLine}" +
                    $"Key: {key}{Environment.NewLine}";
                throw new AmazonS3ClientException(errorMessage, e);
            }
        }

        /// <summary>
        /// Delete a file object from bucket. 
        /// </summary>
        /// <param name="key">This key is used to identify the object in S3</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>GetObjectResponse</returns>
        public async Task<DeleteObjectResponse> DeleteObjectAsync(string key, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            try
            {
                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = _bucketName,
                    Key = key
                };

                var deleteResult = await _client.DeleteObjectAsync(deleteObjectRequest, cancellationToken).ConfigureAwait(false);
                return deleteResult;
            }
            catch (Exception e)
            {
                var errorMessage = $"Error encountered {e.Message}.{Environment.NewLine}" +
                   $"It was not possible to delete the object.{Environment.NewLine}" +
                   $"BucketName: {_bucketName}{Environment.NewLine}" +
                   $"Key: {key}{Environment.NewLine}";
                throw new AmazonS3ClientException(errorMessage, e);
            }
        }

        /// <summary>
        /// Delete a list of objects from bucket. 
        /// </summary>
        /// <param name="keys">List of object keys to delete.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>DeleteObjectsResponse</returns>
        public async Task<DeleteObjectsResponse> DeleteObjectsAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default)
        {
            if (keys.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(keys));
            }

            if (keys.Any(key => string.IsNullOrWhiteSpace(key)))
            {
                throw new Exception("Element keys cannot be null or empty.");
            }

            try
            {
                var deleteObjectsRequest = new DeleteObjectsRequest
                {
                    BucketName = _bucketName,
                    Objects = new List<KeyVersion>(keys.Select(key => new KeyVersion() { Key = key }))
                };

                var deleteResult = await _client.DeleteObjectsAsync(deleteObjectsRequest, cancellationToken).ConfigureAwait(false);
                return deleteResult;
            }
            catch (Exception e)
            {
                var errorMessage = $"Error encountered {e.Message}.{Environment.NewLine}" +
                   $"It was not possible to delete the objects.{Environment.NewLine}" +
                   $"BucketName: {_bucketName}{Environment.NewLine}" +
                   $"Key: {string.Join(",", keys)}{Environment.NewLine}";
                throw new AmazonS3ClientException(errorMessage, e);
            }
        }

        /// <summary>
        /// Check if the file exists or not. 
        /// </summary>
        /// <param name="fileName">List of object keys to delete.</param>
        /// <param name="versionId">Propagates notification that operations should be canceled.</param>
        /// <returns>DeleteObjectsResponse</returns>
        public async Task<bool> IsFileExistsAsync(string fileName, string versionId)
        {
            try
            {
                GetObjectMetadataRequest request = new GetObjectMetadataRequest()
                {
                    BucketName = _bucketName,
                    Key = fileName,
                    VersionId = !string.IsNullOrEmpty(versionId) ? versionId : null
                };
                var response = await _client.GetObjectMetadataAsync(request);
                return true;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException is AmazonS3Exception awsEx)
                {
                    if (string.Equals(awsEx.ErrorCode, "NoSuchBucket"))
                        return false;
                    else if (string.Equals(awsEx.ErrorCode, "NotFound"))
                        return false;
                }
                throw;
            }
        }
    }
}
