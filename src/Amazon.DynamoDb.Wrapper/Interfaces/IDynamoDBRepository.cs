using Amazon.DynamoDBv2.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amazon.DynamoDb.Wrapper.Interfaces
{
    /// <summary>
        /// Provides Amazon DynamoDb operations using Low Level APIs.
        /// See https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/LowLevelDotNetItemsExample.html for more details
        /// </summary>
    public interface IDynamoDBRepository
    {
        /// <summary>
        /// This method provides synchronous write operation that groups up to 100 action requests
        /// The actions are completed atomically so that either all of them succeed, or all of them fail
        /// See https://docs.aws.amazon.com/amazondynamodb/latest/APIReference/API_TransactWriteItems.html for more details
        /// </summary>
        /// <param name="batchRequests">List of TransactWriteItem</param>
        Task RunTransaction(List<TransactWriteItem> transactWriteItems);
        
        /// <summary>
        /// This method puts or deletes multiple items in DynamoDB table in a batch.
        /// A batch can not write more than 25 items in a request, otherwise DynamoDB will reject entire batch write operation.
        /// See https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/batch-operation-lowlevel-dotnet.html for more details.
        /// </summary>
        /// <param name="batchRequests">List of WriteRequest</param>
        Task BatchWrite(Dictionary<string, List<WriteRequest>> batchRequests);

        /// <summary>
        /// This method edits an existing item's attributes, or adds a new item to the table if it does not already exist.
        /// See https://docs.aws.amazon.com/amazondynamodb/latest/APIReference/API_UpdateItem.html for more information
        /// </summary>
        /// <param name="updateItemRequest">UpdateItemRequest object</param>
        Task Update(UpdateItemRequest updateItemRequest);
    }
}
