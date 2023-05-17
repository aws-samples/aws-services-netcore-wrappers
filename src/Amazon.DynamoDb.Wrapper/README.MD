# Getting Started with DynamoDB Repository library
This library is an implementation of repository pattern in C# to connect .NET applications with DynamoDB.

AWS SDK provides 3 ways to programatically connect a .NET application with Amazon DynamoDB.
1. Object Persistence Model
2. DynamoDB Document Model
3. DynamoDB Low Level API	

This library provides implementation for various DynamoDB opearations using **Object Persistence Model** and **DynamoDB Low Level API**.

## Getting Started

### 1. Setup
1. Create a new `ASP.NET Core Web API` project from Visual Studio.
2. Add project reference of `DynamoDB.Repository` project.
3. Go to `program.cs` file and register DynamoDB services like below.
    ```
    // 1. Add required namespace
    using Amazon.DynamoDb.Wrapper.Extensions;

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddControllers();

    // 2. Register DynamoDB services
    builder.Services.RegisterDynamoDBServices(builder.Configuration);

    var app = builder.Build();
    ```
    Here `Configuration` is used to read profile settings from `appsettings.json` file (if defined). 
    ```
    {
      "AWS": {
        "Profile": "local-test-profile",
        "Region": "us-west-2"
      }
    }
    ```  
    Also, add below settings in `appsettings.json` file when you are working with DynamoDB locally.
    ```
    "DynamoDb": {
        "LocalMode": true,
        "LocalServiceUrl": "http://localhost:8000",
        "TableNamePrefix": ""
    }
    ```  
    See [Setting up DynamoDB local](https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/DynamoDBLocal.html) to learn how to setup DynamoDB locally.
4. That's all. Your'e done with necessary setup. 

### 2. Creating Entity classes
Create entity classes, and decorate them with [DynamoDB attributes](https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/DeclarativeTagsList.html).
```
[DynamoDBTable("Blogs")]
public class BaseEntity
{
    [DynamoDBHashKey]
    public string PK { get; set; }

    [DynamoDBRangeKey]
    public string SK { get; set; }
}

public class AuthorEntity : BaseEntity
{
    public string AuthorName { get; set; }
    public string AuthorEmail { get; set; }
}

public class BlogEntity : BaseEntity
{
    public string Title { get; set; }
    public string Content { get; set; }
    public string CreatedDate { get; set; }
    public bool Published { get; set; }
    public int ViewCount { get; set; }
    public string AuthorId { get; set; }
    public string GSI1PK { get; set; }
    public string GSI1SK { get; set; }
    public string GSI2PK { get; set; }
    public string GSI2SK { get; set; }
}

public class GSI1Entity : BaseEntity
{
    [DynamoDBGlobalSecondaryIndexHashKey("GSI1")]
    public string GSI1PK { get; set; }

    [DynamoDBGlobalSecondaryIndexRangeKey("GSI1")]
    public string GSI1SK { get; set; }
}

public class GSI2Entity : BaseEntity
{
    [DynamoDBGlobalSecondaryIndexHashKey("GSI2")]
    public string GSI2PK { get; set; }

    [DynamoDBGlobalSecondaryIndexRangeKey("GSI2")]
    public string GSI2SK { get; set; }
}
```


### 3. About Repository classes
This library consists of 2 repository classes.
1. `IDynamoDBGenericRepository<T>`
2. `IDynamoDBRepository`

**IDynamoDBGenericRepository\<T>** class mostly uses `.NET Object Persistence Model (High Level APIs)` to communicate with DynamoDB. It sometimes uses combination of both `High Level` & `Low Level` APIs to perform an operation.

```
public interface IDynamoDBGenericRepository<TEntity> where TEntity : class
{
    Task<TEntity> GetByPrimaryKey(object partitionKey);
    Task<TEntity> GetByPrimaryKey(object partitionKey, object sortKey);
    Task Save(TEntity entity);
    Task Delete(TEntity entity);
    Task Delete(object partitionKey);
    Task Delete(object partitionKey, object sortKey);
    Task<IEnumerable<TEntity>> Query(QueryOperationConfig queryOperationConfig);
    Task<IEnumerable<TEntity>> Query(QueryFilter filter, bool backwardSearch = false, string indexName = "", List<string>? attributesToGet = null);
    Task<IEnumerable<TEntity>> Scan(ScanFilter filter, List<string>? attributesToGet = null);
    Task<IEnumerable<TEntity>> BatchGet(List<object> partitionKeys);
    Task<IEnumerable<TEntity>> BatchGet(List<Tuple<object, object>> partitionAndSortKeys);
    Task BatchWrite(List<TEntity> entitiesToSave, List<TEntity> entitiesToDelete);
    Task Save(TEntity entity, string conditionExpression);
}
```

**IDynamoDBRepository** class uses DynamoDB `Low Level APIs` to communicate with DynamoDB. This class contains methods for the operations that are not supported by `.NET Object Persistence Model`.
```
public interface IDynamoDBRepository
{
    string GetTableName<T>();
    Task RunTransaction(List<TransactWriteItem> transactWriteItems);
    Task BatchWrite(Dictionary<string, List<WriteRequest>> batchRequests);
    Task Update(UpdateItemRequest updateItemRequest);
}
```
Not everything can be achived with `.NET Object Persistence Model`. For example, this model does not provide APIs to perform `transaction` and `update` operations. That's where, `IDynamoDBRepository` comes into the picture.

### 4. Creating Service class 
Create sevice classes like below. Here, you can use `IDynamoDBGenericRepository<T>` and `IDynamoDBRepository` interfaces. Dependency Injection for these interfaces has already been configured via `RegisterDynamoDBServices` extension method.

#### AuthorService.cs
```
public class AuthorService : IAuthorService
{
    private readonly IDynamoDBGenericRepository<AuthorEntity> _authorRepository;
    private readonly IDynamoDBRepository _dynamoDBRepository;
    private readonly IDynamoDBContext _context;

    private readonly IMapper _mapper;

    public AuthorService(IDynamoDBGenericRepository<AuthorEntity> authorRepository,
        IDynamoDBRepository dynamoDBRepository, IMapper mapper, IDynamoDBContext context)
    {
        _authorRepository = authorRepository;
        _dynamoDBRepository = dynamoDBRepository;
        _mapper = mapper;
        _context = context;
    }
  

    // Add additional methods here
}
```
#### BlogService.cs
```
    public class BlogService : IBlogService
    {
        private readonly IDynamoDBGenericRepository<BlogEntity> _blogRepository;
        private readonly IDynamoDBGenericRepository<GSI1Entity> _gsi1Repository;
        private readonly IDynamoDBGenericRepository<GSI2Entity> _gsi2Repository;
        private readonly IDynamoDBRepository _dynamoDBRepository;
        private readonly IDynamoDBContext _context;

        private readonly IMapper _mapper;

        public BlogService(IDynamoDBGenericRepository<BlogEntity> blogRepository, IDynamoDBRepository dynamoDBRepository,
            IDynamoDBGenericRepository<GSI1Entity> gsi1Repository, IDynamoDBGenericRepository<GSI2Entity> gsi2Repository,
        IMapper mapper, IDynamoDBContext context)
        {
            _blogRepository = blogRepository;
            _gsi1Repository = gsi1Repository;
            _gsi2Repository = gsi2Repository;
            _dynamoDBRepository = dynamoDBRepository;
            _mapper = mapper;
            _context = context;
        }
        
        // Add additional methods here
```

### 5. Operation examples
These examples will help you to in understanding, how to use this library to perform various common operations on a DynamoDB table.

##### Load
```
public async Task<AuthorDTO> GetAuthorById(string authorId)
{
    var authorEntity = await _authorRepository.GetByPrimaryKey(AppConstants.AUTHOR_PARTITION_KEY, GetSortKey(authorId));
    return _mapper.Map<AuthorDTO>(authorEntity);
}
```

##### Save
```
public async Task SaveAuthor(AuthorDTO author)
{
    var authorEntity = _mapper.Map<AuthorEntity>(author);
    await _authorRepository.Save(authorEntity);
}
```

##### Save with condition expression
```
public async Task SaveAuthor(AuthorDTO author)
{
    var authorEntity = _mapper.Map<AuthorEntity>(author);
    await _authorRepository.Save(authorEntity, $"attribute_not_exists({nameof(AuthorEntity.PK)})");
}
```

##### Delete
```
public async Task DeleteAuthor(string authorId)
{
    await _authorRepository.Delete(AppConstants.AUTHOR_PARTITION_KEY, GetSortKey(authorId));
}
```
##### Query
```
public async Task<List<AuthorDTO>> GetAuthorList()
{
    var filter = new QueryFilter();
    filter.AddCondition(nameof(BaseEntity.PK), QueryOperator.Equal, AppConstants.AUTHOR_PARTITION_KEY);

    var authorList = await _authorRepository.Query(filter);
    return _mapper.Map<List<AuthorDTO>>(authorList);
}
```
##### Scan
```
public async Task<List<BlogDTO>> GetBlogsWithMoreThan1000Views()
{
    var scanfilter = new ScanFilter();
    scanfilter.AddCondition(nameof(BlogEntity.ViewCount), ScanOperator.GreaterThan, 1000);

    var blogList = await _blogRepository.Scan(scanfilter);
    return _mapper.Map<List<BlogDTO>>(blogList);
}
```
##### Query GSI
```
// 1. Prepare query filter, add GSI PK & SK in conditions
var queryFilter = new QueryFilter();
queryFilter.AddCondition(nameof(BlogEntity.GSI2PK), QueryOperator.Equal, AppConstants.BLOG_PARTITION_KEY);

// 2. Define attributes to get, as all attributes won't work in GSI query
List<string> attributesToGet = new List<string>();
foreach (PropertyInfo prop in typeof(GSI2Entity).GetProperties())
{
    attributesToGet.Add(prop.Name);
}

// 3. Finally hit the query, here we get all blog ids sorted by date
var gsi2Entities = await _gsi2Repository.Query(queryFilter, backwardSearch: true, indexName: AppConstants.GSI2_INDEX_NAME, attributesToGet: attributesToGet);

```
##### Update
```
public void UpdateViewCount(string blogId)
{

    var updateItemRequest = new UpdateItemRequest
    {
        TableName = _dynamoDBRepository.GetTableName<BlogEntity>(),
        Key = new Dictionary<string, AttributeValue>
        {
            { nameof(BlogEntity.PK), new AttributeValue { S = AppConstants.BLOG_PARTITION_KEY } },
            {  nameof(BlogEntity.SK), new AttributeValue { S =  $"{AppConstants.BLOG_PARTITION_KEY}{AppConstants.DELIMITER}{blogId}"} }
        },
        UpdateExpression = "SET ViewCount = ViewCount + :incr",
        ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
        {
            {
                ":incr", new AttributeValue { N = "1" }
            }
        }
    };
    _dynamoDBRepository.Update(updateItemRequest);
}
```

##### Transaction
```
// 1. Prepare transaction request
List<TransactWriteItem> transactWriteItems = new List<TransactWriteItem>();

foreach (var item in baseEntities)
{
    Dictionary<string, AttributeValue> attributes = new();

    if (item is BlogEntity blogEntity)
    {
        attributes = _context.ToDocument(blogEntity).ToAttributeMap();
    }
    else if (item is AuthorEntity authorEntity)
    {
        attributes = _context.ToDocument(authorEntity).ToAttributeMap();
    }

    transactWriteItems.Add(new TransactWriteItem
    {
        Put = new Put
        {
            Item = attributes,
            TableName = _dynamoDBRepository.GetTableName<BaseEntity>()
        }
    });
}

// 2. Execute transaction request
await _dynamoDBRepository.RunTransaction(transactWriteItems);
```

##### Batch Write
Example of Batch Write. First getting items to delete, then deleting them in a batch write request.
```
// 1. Getting old records (as currently, You cannot delete all the items just by passing the Hash key, so we first have to retrive them to know their pk & sk)
var filter = new QueryFilter();
filter.AddCondition(nameof(BlogEntity.PK).ToLower(), QueryOperator.Equal, AppConstants.BLOG_PARTITION_KEY);
filter.AddCondition(nameof(BlogEntity.SK), QueryOperator.BeginsWith, AppConstants.BLOG_PARTITION_KEY);

var oldBlogs = await _blogRepository.Query(filter);

// 2. Create bacth request to delete old records
Dictionary<string, List<WriteRequest>> batchRequests = new();

List<WriteRequest> writeRequests = new();
foreach (var item in oldBlogs)
{
    var primaryKey = new Dictionary<string, AttributeValue>
    {
        { nameof(BlogEntity.PK).ToLower(), new AttributeValue(item.PK) },
        { nameof(BlogEntity.SK).ToLower(), new AttributeValue(item.SK) }
    };

    writeRequests.Add(new WriteRequest
    {
        DeleteRequest = new DeleteRequest
        {
            Key = primaryKey
        }
    });
}

// 3. Execute batch
if (writeRequests.Any())
{
    string tableName =  _dynamoDBRepository.GetTableName<BaseEntity>();

    batchRequests.Add(tableName, writeRequests);

    await _dynamoDBRepository.BatchWrite(batchRequests);
}
```

## IAM Permissions required for the operations

a) IAM Permission required for all the operations are:

```
{
    "Version": "2012-10-17",
    "Statement": [
        {
            "Sid": "VisualEditor0",
            "Effect": "Allow",
            "Action": [
                "dynamodb:BatchGetItem",
                "dynamodb:BatchWriteItem",
                "dynamodb:PutItem",
                "dynamodb:DeleteItem",
                "dynamodb:GetItem",
                "dynamodb:Scan",
                "dynamodb:Query",
                "dynamodb:UpdateItem"
            ],
            "Resource": "arn:aws:dynamodb:<Region>:<AWSAccount>:table/<TableName>"
        }
    ]
}
```

## Sample application
Refer this [sample Blog API application](../../samples/dynamodb-sample-app) written in ASP.NET 6 to explain how to use DynamoDB repository in real-world application.
