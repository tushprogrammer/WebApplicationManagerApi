using ApplicationManager_ClassLibrary.Entitys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplicationManagerApi.ContextFolder;

namespace WebApplicationManagerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly ApplicationDbContext Context;
        public ServiceController(ApplicationDbContext context)
        {
            Context = context;
        }

        [HttpGet]
        public IQueryable<Service> GetServices()
        {
            return Context.Services;
        }
    }
}
