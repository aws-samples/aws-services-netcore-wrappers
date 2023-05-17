using BlogApplication.DTO;
using BlogApplication.Service;
using Microsoft.AspNetCore.Mvc;

namespace BlogApplication.API.Controllers
{
    [Route("api/author")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        public AuthorController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> SaveAuthor(AuthorDTO author)
        {
            await _authorService.SaveAuthor(author);
            return Ok();
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeleteAuthor(string id)
        {
            await _authorService.DeleteAuthor(id);
            return Ok();
        }


        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetAuthorById(string id)
        {
            var author = await _authorService.GetAuthorById(id);
            return Ok(author);
        }

        [HttpGet]
        [Route("list")]
        public async Task<IActionResult> GetAuthorList()
        {
            var authorList = await _authorService.GetAuthorList();
            return Ok(authorList);
        }
    }
}
