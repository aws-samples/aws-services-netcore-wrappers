using Amazon.DynamoDBv2.DataModel;

namespace BlogApplication.Repository.Entities
{
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
}
