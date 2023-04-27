using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amazon.DynamoDb.Wrapper.Interfaces
{
    /// <summary>
    /// Interface to provide DynamoDB Operations. This uses object persistence model that enables you to map your client-side classes to Amazon DynamoDB tables
    /// </summary>
    public interface IDynamoDBGenericRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// Retrieves an item from a table. It uses the primary key made up of Partition key
        ///The method requires only the primary key of the item you want to retrieve.
        /// </summary>
        /// <param name="partitionKey">partitionKey of the item to be retrieved
        /// <param name="cancellationToken"> A cancellation token that can be used by other objects or threads to receive
        ///     notice of cancellation.</param>
        /// <returns>Returns the entity retrieved from Amazon DynamoDB</returns>
        Task<TEntity> GetByPrimaryKey(object partitionKey);

        /// <summary>
        /// Retrieves an item from a table. It uses the primary key made up of Partition key and sort key
        ///The method requires only the primary key of the item you want to retrieve.
        /// </summary>
        /// <param name="partitionKey">partitionKey of the item to be retrieved
        /// <param name="sortKey">sortKey of the item to be retrieved
        /// <returns>Returns the entity retrieved from Amazon DynamoDB</returns>
        Task<TEntity> GetByPrimaryKey(object partitionKey, object sortKey);
        
        /// <summary>
        /// Saves the specified object in the table
        /// If the primary key specified in the input object doesn't exist in the table, the method adds a new item to the table.
        /// </summary>
        /// <param name="entity">entity to be created/updated in the table
        Task Save(TEntity entity);

        /// <summary>
        /// Deletes an item from the table
        /// The method requires the primary key of the item in the entity object you want to delete. 
        /// </summary>
        /// <param name="entity">entity to be created/updated in the table
        Task Delete(TEntity entity);

        /// <summary>
        /// Deletes an item from the table
        /// </summary>
        /// <param name="partitionKey">partition key of the item to be deleted
        Task Delete(object partitionKey);

        /// <summary>
        /// Deletes an item from the table
        /// </summary>
        /// <param name="partitionKey">partitionKey of the item to be deleted
        /// <param name="sortKey">sortKey of the item to be deleted
        Task Delete(object partitionKey, object sortKey);

        /// <summary>
        /// Saves the specified object to the Amazon DynamoDB table
        /// Queries a table based on query parameters you provide. 
        /// You can query a table only if it has a composite primary key (partition key and sort key). 
        /// When querying, you must specify a partition key and a condition that applies to the sort key.
        /// </summary>
        /// <param name="queryOperationConfig">queryOperationConfig object containing QueryFilter and/or Expression to be used for querying the table
        /// <returns>Returns the IEnumerable containing one or more entities retrieved from DynamoDB</returns>
        Task<IEnumerable<TEntity>> Query(QueryOperationConfig queryOperationConfig);
        
        /// <summary>
        /// Saves the specified object to the Amazon DynamoDB table
        /// Queries a table based on query parameters you provide. 
        /// You can query a table only if it has a composite primary key (partition key and sort key). 
        /// When querying, you must specify a partition key and a condition that applies to the sort key.
        /// </summary>
        /// <param name="QueryFilter">QueryFilter object containing QueryFilter to be used for querying the table
        /// <param name="backwardSearch">Flag to signal if the search needs to traverse backwards
        /// <param name="indexName">Optional index name if search needs to be against the index
        /// <param name="attributesToGet">Optional list of attributesToGet
        /// <returns>Returns the IEnumerable containing one or more entities retrieved from DynamoDB</returns>
        Task<IEnumerable<TEntity>> Query(QueryFilter filter, bool backwardSearch = false, string indexName = "", List<string>? attributesToGet = null);
        
        /// <summary>
        /// Performs an entire table scan.
        // You can filter scan results by specifying a scan condition. 
        //The condition can be evaluated on any attributes in the table.
        /// </summary>
        /// <param name="filter">filter to be used for scan
        /// <param name="attributesToGet">Optional list of attributesToGet
        /// <returns>Returns the IEnumerable containing one or more entities retrieved from DynamoDB</returns>
        Task<IEnumerable<TEntity>> Scan(ScanFilter filter, List<string>? attributesToGet = null);
        
        /// <summary>
        /// Gets the items from the table in a batch.
        /// A batch get operation can not return more than 100 items in a request, otherwise DynamoDB will reject entire batch operation.
        /// </summary>
        /// <param name="partitionKeys">List of partitionKeys of the items to be retrieved
        /// <returns>Returns the IEnumerable containing one or more entities retrieved from DynamoDB</returns>
        Task<IEnumerable<TEntity>> BatchGet(List<object> partitionKeys);
        
        /// <summary>
        /// Gets the items from the table in a batch.
        /// A batch get operation can not return more than 100 items in a request, otherwise DynamoDB will reject entire batch operation.
        /// </summary>
        /// <param name="partitionAndSortKeys">List of tuple containing partition and sort keys of the items to be retrieved
        /// <returns>Returns the IEnumerable containing one or more entities retrieved from DynamoDB</returns>
        Task<IEnumerable<TEntity>> BatchGet(List<Tuple<object, object>> partitionAndSortKeys);
        
        /// <summary>
        /// This method puts or deletes multiple items in DynamoDB table in a batch.
        /// A batch can not write more than 25 items in a request, otherwise DynamoDB will reject entire batch write operation.
        /// </summary>
        /// <param name="entitiesToSave">Entities to put</param>
        /// <param name="entitiesToDelete">Entities to delete</param>
        Task BatchWrite(List<TEntity> entitiesToSave, List<TEntity> entitiesToDelete);
        
        /// <summary>
        /// Saves the specified object based on conditionExpression to the table
        /// </summary>
        /// <param name="entity">entity to be created/updated in the table
        /// <param name="conditionExpression">conditionExpression to specify that the item can be replaced only if the existing item has the ISBN attribute with a specific value </param>
        Task Save(TEntity entity, string conditionExpression);
    }
}
