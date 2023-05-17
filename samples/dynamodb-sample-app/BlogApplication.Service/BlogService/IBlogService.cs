using BlogApplication.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlogApplication.Service
{
    public interface IBlogService
    {
        Task SaveBlog(BlogDTO blog);
        Task<List<BlogDTO>> GetBlogList();
        Task<List<BlogDTO>> GetBlogListByAuthor(string authorId);
        Task<BlogDTO> GetBlogById(string id);
        Task Delete(string id);
    }
}
