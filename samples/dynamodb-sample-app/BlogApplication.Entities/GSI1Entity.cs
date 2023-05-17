using Amazon.DynamoDBv2.DataModel;

namespace BlogApplication.Repository.Entities
{
    public class GSI1Entity : BaseEntity
    {
        [DynamoDBGlobalSecondaryIndexHashKey("GSI1")]
        public string GSI1PK { get; set; }

        [DynamoDBGlobalSecondaryIndexRangeKey("GSI1")]
        public string GSI1SK { get; set; }
    }
}
