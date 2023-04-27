using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDb.Wrapper.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDb.Wrapper.Extensions;

namespace Amazon.DynamoDb.Wrapper.Implementations
{
    /*
    * Note:
    *
    * ****** Important Methods of DynamoDBContext *******
    * 1. Load: Retrieves an item from a table. The method requires only the primary key of the item you want to retrieve.
    * 2. Query: Queries a table based on query parameters you provide. You can query a table only if it has a composite primary key (partition key and sort key). When querying, you must specify a partition key and a condition that applies to the sort key.
    * 3. Scan: Performs an entire table scan. You can filter scan results by specifying a scan condition. The condition can be evaluated on any attributes in the table.
    * 4. Save: Saves the specified object to the table. If the primary key specified in the input object doesn't exist in the table, the method adds a new item to the table. If the primary key exists, the method updates the existing item.
    * 5. Delete: Deletes an item from the table. The method requires the primary key of the item you want to delete. You can provide either the primary key value or a client-side object containing a primary key value as a parameter to this method.
    * 6. FromQuery: Runs a Query operation, with the query parameters defined in a QueryOperationConfig object.
    * 7. FromScan: Runs a Scan operation, with the scan parameters defined in a ScanOperationConfig object.
    * Reference: https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/DotNetDynamoDBContext.html
    */

    public class DynamoDBGenericRepository<TEntity> : IDynamoDBGenericRepository<TEntity> where TEntity : class
    {
        private readonly IDynamoDBContext _context;
        private readonly IAmazonDynamoDB _client;
        private readonly IDynamoDBRepository _repository;

        public DynamoDBGenericRepository(IDynamoDBContext context, IAmazonDynamoDB client, IDynamoDBRepository repository)
        {
            _context = context;
            _client = client;
            _repository = repository;
        }

        #region High Level API (Object Persistence Model)
        public async Task<TEntity> GetByPrimaryKey(object partitionKey)
        {
            return await _context.LoadAsync<TEntity>(partitionKey);
        }

        public async Task<TEntity> GetByPrimaryKey(object partitionKey, object sortKey)
        {
            return await _context.LoadAsync<TEntity>(partitionKey, sortKey);
        }

        public async Task Save(TEntity entity)
        {
            await _context.SaveAsync(entity);
        }

        public async Task Delete(TEntity entity)
        {
            await _context.DeleteAsync(entity);
        }

        public async Task Delete(object partitionKey)
        {
            await _context.DeleteAsync<TEntity>(partitionKey);
        }

        public async Task Delete(object partitionKey, object sortKey)
        {
            await _context.DeleteAsync<TEntity>(partitionKey, sortKey);
        }

        public async Task<IEnumerable<TEntity>> Query(QueryOperationConfig queryOperationConfig)
        {
            var search = _context.FromQueryAsync<TEntity>(queryOperationConfig);
            var items = new List<TEntity>();
            do
            {
                var nextSet = await search.GetNextSetAsync();
                items.AddRange(nextSet);
            }
            while (!search.IsDone);

            return items;
        }

        public async Task<IEnumerable<TEntity>> Query(QueryFilter filter, bool backwardSearch = false, string indexName = "", List<string>? attributesToGet = null)
        {
            var queryConfig = new QueryOperationConfig();

            if (attributesToGet != null && attributesToGet.Any())
            {
                queryConfig.Select = SelectValues.SpecificAttributes;
                queryConfig.AttributesToGet = attributesToGet;
            }
            else
            {
                queryConfig.Select = SelectValues.AllAttributes;
            }

            if (!string.IsNullOrWhiteSpace(indexName))
            {
                queryConfig.IndexName = indexName;
            }

            queryConfig.BackwardSearch = backwardSearch;
            queryConfig.Filter = filter;

            var search = _context.FromQueryAsync<TEntity>(queryConfig);
            var items = new List<TEntity>();
            do
            {
                var nextSet = await search.GetNextSetAsync();
                items.AddRange(nextSet);
            }
            while (!search.IsDone);

            return items;
        }

        public async Task<IEnumerable<TEntity>> Scan(ScanFilter filter, List<string>? attributesToGet = null)
        {
            var scanConfig = new ScanOperationConfig();
            scanConfig.Filter = filter;

            if (attributesToGet != null && attributesToGet.Any())
            {
                scanConfig.Select = SelectValues.SpecificAttributes;
                scanConfig.AttributesToGet = attributesToGet;
            }
            else
            {
                scanConfig.Select = SelectValues.AllAttributes;
            }

            var search = _context.FromScanAsync<TEntity>(scanConfig);

            var items = new List<TEntity>();
            do
            {
                var nextSet = await search.GetNextSetAsync();
                items.AddRange(nextSet);
            }
            while (!search.IsDone);

            return items;
        }

        /// <summary>
        /// A batch get operation can not return more than 100 items in a request, otherwise DynamoDB will reject entire batch operation.
        /// </summary>
        public async Task<IEnumerable<TEntity>> BatchGet(List<object> partitionKeys)
        {
            var batchRequest = _context.CreateBatchGet<TEntity>();

            // add delete requests in batch
            if (partitionKeys != null && partitionKeys.Any())
            {
                foreach (var item in partitionKeys)
                {
                    batchRequest.AddKey(item);
                }
            }

            await batchRequest.ExecuteAsync();

            return batchRequest.Results;
        }

        /// <summary>
        /// A batch get operation can not return more than 100 items in a request, otherwise DynamoDB will reject entire batch operation.
        /// </summary>
        public async Task<IEnumerable<TEntity>> BatchGet(List<Tuple<object, object>> partitionAndSortKeys)
        {
            var batchRequest = _context.CreateBatchGet<TEntity>();

            // add delete requests in batch
            if (partitionAndSortKeys != null && partitionAndSortKeys.Any())
            {
                foreach (var item in partitionAndSortKeys)
                {
                    batchRequest.AddKey(item.Item1, item.Item2);
                }
            }

            await batchRequest.ExecuteAsync();

            return batchRequest.Results;
        }

        /// <summary>
        /// This method puts or deletes multiple items in DynamoDB table in a batch.
        /// A batch can not write more than 25 items in a request, otherwise DynamoDB will reject entire batch write operation.
        /// </summary>
        /// <param name="entitiesToSave">Entities to put</param>
        /// <param name="entitiesToDelete">Entities to delete</param>
        public async Task BatchWrite(List<TEntity> entitiesToSave, List<TEntity> entitiesToDelete)
        {
            var batchRequest = _context.CreateBatchWrite<TEntity>();

            // add save requests in batch
            batchRequest.AddPutItems(entitiesToSave);

            // add delete requests in batch
            if (entitiesToDelete != null && entitiesToDelete.Any())
            {
                entitiesToDelete.ForEach(item =>
                {
                    batchRequest.AddDeleteKey(item);
                });
            }

            await batchRequest.ExecuteAsync();
        }
        #endregion

        #region Low Level API + High Level API (Hybrid)
        public async Task Save(TEntity entity, string conditionExpression = "")
        {
            var document = _context.ToDocument(entity);
            var attributeMaps = document.ToAttributeMap();

            // retrieves the target table for the specified type.
            var tableName = GetTableName<TEntity>();

            PutItemRequest putItemRequest = new PutItemRequest
            {
                TableName = tableName,
                Item = attributeMaps,
            };

            if (!string.IsNullOrWhiteSpace(conditionExpression))
            {
                putItemRequest.ConditionExpression = conditionExpression;
            }

            var response = await _client.PutItemAsync(putItemRequest);

            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"Operation failed with status code {response.HttpStatusCode}");
            }
        }

        private string GetTableName<T>()
        {
            var tableNameAttribute = typeof(T).GetCustomAttributes(typeof(DynamoDBTableAttribute), true).FirstOrDefault() as DynamoDBTableAttribute;

            if (tableNameAttribute == null)
            {
                throw new Exception("DynamoDBTableAttribute not found");
            }

            string prefix = !string.IsNullOrWhiteSpace(StartupExtentions.tableNamePrefix) ? StartupExtentions.tableNamePrefix : string.Empty;

            return $"{prefix}{tableNameAttribute.TableName}";
        }
        #endregion
    }
}