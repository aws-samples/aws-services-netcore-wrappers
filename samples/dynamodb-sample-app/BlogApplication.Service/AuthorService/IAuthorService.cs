using BlogApplication.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlogApplication.Service
{
    public interface IAuthorService
    {
        Task SaveAuthor(AuthorDTO author);
        Task DeleteAuthor(string authorId);
        Task<AuthorDTO> GetAuthorById(string authorId);
        Task<List<AuthorDTO>> GetAuthorList();
    }
}
