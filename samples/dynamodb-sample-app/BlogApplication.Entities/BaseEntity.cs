using Amazon.DynamoDBv2.DataModel;

namespace BlogApplication.Repository.Entities
{
    [DynamoDBTable("Blogs")]
    public class BaseEntity
    {
        [DynamoDBHashKey]
        public string PK { get; set; }

        [DynamoDBRangeKey]
        public string SK { get; set; }
    }
}
