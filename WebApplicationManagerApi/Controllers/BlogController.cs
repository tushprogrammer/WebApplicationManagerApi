using ApplicationManager_ClassLibrary.Entitys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplicationManagerApi.ContextFolder;

namespace WebApplicationManagerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly ApplicationDbContext Context;
        public BlogController(ApplicationDbContext context)
        {
            Context = context;
        }

        [HttpGet]
        public IQueryable<Blog> GetBlogs()
        {
            return Context.Blogs;
        }
    }
}
