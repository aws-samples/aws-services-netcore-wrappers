using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDb.Wrapper.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amazon.DynamoDb.Wrapper.Implementations
{
    public class DynamoDBRepository : IDynamoDBRepository
    {
        private readonly IAmazonDynamoDB _client;

        public DynamoDBRepository(IAmazonDynamoDB client)
        {
            _client = client;
        }

        #region Low Level API

        /// <summary>
        /// This method provides synchronous write operation that groups up to 100 action requests
        /// The actions are completed atomically so that either all of them succeed, or all of them fail
        /// See https://docs.aws.amazon.com/amazondynamodb/latest/APIReference/API_TransactWriteItems.html for more details
        /// </summary>
        /// <param name="batchRequests">List of TransactWriteItem</param>
        public async Task RunTransactionAsync(List<TransactWriteItem> transactWriteItems)
        {
            var transactWriteItemsRequest = new TransactWriteItemsRequest();
            transactWriteItemsRequest.TransactItems = transactWriteItems;

            var response = await _client.TransactWriteItemsAsync(transactWriteItemsRequest);

            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"Operation failed with status code {response.HttpStatusCode}");
            }
        }

        /// <summary>
        /// This method puts or deletes multiple items in DynamoDB table in a batch.
        /// A batch can not write more than 25 items in a request, otherwise DynamoDB will reject entire batch write operation.
        /// See https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/batch-operation-lowlevel-dotnet.html for more details.
        /// </summary>
        /// <param name="batchRequests">List of WriteRequest</param>
        public async Task BatchWriteAsync(Dictionary<string, List<WriteRequest>> batchRequests)
        {
            var batchWriteItemsRequest = new BatchWriteItemRequest();
            batchWriteItemsRequest.RequestItems = batchRequests;

            var response = await _client.BatchWriteItemAsync(batchWriteItemsRequest);

            if (response.UnprocessedItems.Count > 0)
            {
                throw new Exception($"{response.UnprocessedItems.Count} items failed during batch write operation");
            }
            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"Operation failed with status code {response.HttpStatusCode}");
            }
        }

        /// <summary>
        /// This method edits an existing item's attributes, or adds a new item to the table if it does not already exist.
        /// See https://docs.aws.amazon.com/amazondynamodb/latest/APIReference/API_UpdateItem.html for more information
        /// </summary>
        /// <param name="updateItemRequest">UpdateItemRequest object</param>
        public async Task UpdateAsync(UpdateItemRequest updateItemRequest)
        {
            var response = await _client.UpdateItemAsync(updateItemRequest);

            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"Operation failed with status code {response.HttpStatusCode}");
            }
        }
        #endregion
    }
}