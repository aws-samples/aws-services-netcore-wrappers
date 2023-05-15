using BlogApplication.DTO;
using BlogApplication.Service;
using Microsoft.AspNetCore.Mvc;

namespace BlogApplication.API.Controllers
{
    [ApiController]
    [Route("api/blog")]
    public class BlogController : ControllerBase
    {
        private readonly IBlogService _blogService;

        public BlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> SaveBlog(BlogDTO blog)
        {
            await _blogService.SaveBlog(blog);
            return Ok();
        }

        [HttpGet]
        [Route("list")]
        public async Task<IActionResult> GetBlogList()
        {
            var blogList = await _blogService.GetBlogList();
            return Ok(blogList);
        }

        [HttpGet]
        [Route("list/author/{authorId}")]
        public async Task<IActionResult> GetBlogListByAuthor(string authorId)
        {
            var blogList = await _blogService.GetBlogListByAuthor(authorId);
            return Ok(blogList);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetBlogById(string id)
        {
            var blog = await _blogService.GetBlogById(id);
            return Ok(blog);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _blogService.Delete(id);
            return Ok();
        }
    }
}