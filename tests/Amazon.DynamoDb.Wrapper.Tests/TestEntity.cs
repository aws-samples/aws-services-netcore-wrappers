using Amazon.DynamoDBv2.DataModel;

namespace Amazon.DynamoDb.Wrapper.Tests
{
    [DynamoDBTable("TestTable")]
    public class TestEntity
    {
        [DynamoDBHashKey]
        public string PK { get; set; }

        [DynamoDBRangeKey]
        public string SK { get; set; }

        public string Prop_1 { get; set; }
        public string Prop_2 { get; set; }
    }
}
