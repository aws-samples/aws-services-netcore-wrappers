using System.ComponentModel.DataAnnotations;

namespace BlogApplication.DTO
{
    public class AuthorDTO
    {
        [Required]
        public string AuthorId { get; set; }

        [Required]
        public string AuthorName { get; set; }

        [Required]
        [EmailAddress]
        public string AuthorEmail { get; set; }
    }
}
