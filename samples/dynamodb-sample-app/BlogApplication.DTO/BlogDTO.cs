using System;
using System.ComponentModel.DataAnnotations;

namespace BlogApplication.DTO
{
    public class BlogDTO
    {
        [Required]
        public string BlogId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public bool Published { get; set; }

        [Required]
        public string AuthorId { get; set; }
    }
}
