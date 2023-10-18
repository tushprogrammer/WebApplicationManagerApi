using ApplicationManager_ClassLibrary.Entitys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplicationManagerApi.ContextFolder;

namespace WebApplicationManagerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly ApplicationDbContext Context;
        public ContactsController(ApplicationDbContext context)
        {
            Context = context;
        }
        [HttpGet]
        public IQueryable<Contacts> GetContacts()
        {
            return Context.Contacts;
        }

        [Route("GetSocialNet")]
        [HttpGet]
        public IQueryable<SocialNet> GetSocialNet()
        {
            return Context.SocialNets;
        }
    }
}
