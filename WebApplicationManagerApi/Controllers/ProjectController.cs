using ApplicationManager_ClassLibrary.Entitys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplicationManagerApi.ContextFolder;

namespace WebApplicationManagerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : Controller
    {
        private readonly ApplicationDbContext Context;
        public ProjectController(ApplicationDbContext context)
        {
            Context = context;
        }

        [HttpGet]
        public IQueryable<Project> GetProjects()
        {
            return Context.Projects;
        }
    }
}
