using ApplicationManager_ClassLibrary.Entitys;
using Microsoft.AspNetCore.Mvc;
using WebApplicationManagerApi.ContextFolder;

namespace WebApplicationManagerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MainController : ControllerBase
    {

        private readonly ApplicationDbContext Context;
        private readonly ILogger<MainController> _logger;

        public MainController(ILogger<MainController> logger, ApplicationDbContext context)
        {
            Context = context;
            _logger = logger;
        }

        [Route("GetMains")]
        [HttpGet(Name = "GetMains")]
        public IEnumerable<MainPage> GetMains()
        {
            return Context.MainPage;
        }

        [Route("GetMainsIndexPage")]
        [HttpGet]
        public MainForm GetMainsIndexPage()
        {
            // Butt_main, Title, Image_main, Main_request
            IQueryable<MainPage> data = Context.MainPage.Where(item => item.Id >= 6 && item.Id <= 9);
            MainForm form = new()
            {
                ButtonTitle = data.First(i => i.Id == 6).Value,
                Title = data.First(i => i.Id == 7).Value,
                UrlImage = data.First(i => i.Id == 8).Value,
                RequestTitle = data.First(i => i.Id == 6).Value,
            };
            return form;
        }
        [Route("GetMainsAdmin")]
        [HttpGet]
        public IQueryable<MainPage> GetMainsAdmin()
        {
            //MainAdmin, ProjectAdmin, ServicesAdmin, BlogsAdmin, ContactsAdmin, Index

            return Context.MainPage.Where(i => i.Id >= 13 && i.Id <= 18);
        }
        [Route("GetMainsHeader")] 
        [HttpGet]
        public IQueryable<MainPage> GetMainsHeader()
        {
            return Context.MainPage.Where(item => item.Id >= 2 && item.Id <= 5);
        }
    }
}