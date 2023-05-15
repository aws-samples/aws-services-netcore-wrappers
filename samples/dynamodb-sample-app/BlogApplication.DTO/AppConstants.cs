namespace BlogApplication.DTO
{
    public class AppConstants
    {
        public const string BLOG_PARTITION_KEY = "BLOG";
        public const string AUTHOR_PARTITION_KEY = "AUTHOR";
        public const string DELIMITER = "#";

        public const string GSI1_INDEX_NAME = "BlogsByAuthor";
        public const string GSI2_INDEX_NAME = "BlogsByCreatedDate";

        public const string ISO_8601_DATE_FORMAT = "yyyy-MM-dd";
    }
}
