using Amazon.DynamoDBv2.DataModel;

namespace BlogApplication.Repository.Entities
{
    public class GSI2Entity : BaseEntity
    {
        [DynamoDBGlobalSecondaryIndexHashKey("GSI2")]
        public string GSI2PK { get; set; }

        [DynamoDBGlobalSecondaryIndexRangeKey("GSI2")]
        public string GSI2SK { get; set; }
    }
}
