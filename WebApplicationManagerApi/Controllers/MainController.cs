using ApplicationManager_ClassLibrary.Entitys;
using Microsoft.AspNetCore.Mvc;
using System.Net;
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
        [Route("GetRequests")]
        [HttpGet]
        public IQueryable<Request> GetRequests()
        {
            List<Request> tempRequests = Context.Requests.ToList();
            SetStatusRequests(ref tempRequests);
            return tempRequests.AsQueryable();
        }
        private List<Request> SetStatusRequests(ref List<Request> requests)
        {
            List<StatusRequest> StatusRequests = Context.Statuses.ToList();

            foreach (Request item in requests)
            {
                item.Status = StatusRequests.First(i => i.Id == item.StatusId);
            }

            return requests;
        }

        [HttpGet]
        public IQueryable<Request> GetRequests([FromQuery] DateTime DateFor, [FromQuery] DateTime DateTo)
        {
            List<Request> tempRequests =
                Context.Requests.Where(i => i.DateCreated.Date >= DateFor.Date && i.DateCreated.Date <= DateTo.Date).ToList();
            SetStatusRequests(ref tempRequests);
            return tempRequests.AsQueryable();
        }
        [HttpGet]
        public IQueryable<Request> GetRequests([FromBody] string statusName)
        {
            List<Request> tempRequests;
            if (statusName != string.Empty && statusName != null)
            {
                StatusRequest Status = Context.Statuses.First(s => s.StatusName == statusName);
                tempRequests = Context.Requests.Where(i => i.StatusId == Status.Id).ToList();
            }
            else
                tempRequests = Context.Requests.ToList();
            SetStatusRequests(ref tempRequests);
            return tempRequests.AsQueryable();
        }
        [Route("GetRequestsToday")]
        [HttpGet]
        public IQueryable<Request> GetRequestsToday()
        {
            List<Request> tempRequests = Context.Requests.Where(i => i.DateCreated.Date == DateTime.Today).ToList();
            SetStatusRequests(ref tempRequests);
            return tempRequests.AsQueryable();
        }
        [Route("GetRequestsYesterday")]
        [HttpGet]
        public IQueryable<Request> GetRequestsYesterday()
        {
            DateTime yesterday = DateTime.Now.AddDays(-1);
            List<Request> tempRequests =
                Context.Requests.Where(i => i.DateCreated.Date == yesterday).ToList();
            SetStatusRequests(ref tempRequests);
            return tempRequests.AsQueryable();
        }
        [Route("GetRequestsWeek")]
        [HttpGet]
        public IQueryable<Request> GetRequestsWeek()
        {
            int daynow;
            if (DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
                daynow = 7;
            else
                daynow = (int)DateTime.Today.DayOfWeek;
            DateTime firstdayweek = DateTime.Today.AddDays(-daynow + 1);
            DateTime lastdayweek = firstdayweek.AddDays(7);
            List<Request> tempRequests =
                Context.Requests.Where(i => i.DateCreated >= firstdayweek && i.DateCreated <= lastdayweek).ToList();
            SetStatusRequests(ref tempRequests);
            return tempRequests.AsQueryable();
        }
        [Route("GetRequestsMonth")]
        [HttpGet]
        public IQueryable<Request> GetRequestsMonth()
        {
            DateTime now = DateTime.Now;
            DateTime firstdaymonth = new DateTime(now.Year, now.Month, 1);
            DateTime lastdaymonth = new DateTime(now.Year, now.Month + 1, 1).AddDays(-1);
            List<Request> tempRequests =
                Context.Requests.Where(i => i.DateCreated >= firstdaymonth && i.DateCreated <= lastdaymonth).ToList();
            SetStatusRequests(ref tempRequests);
            return tempRequests.AsQueryable();
        }
        [Route("GetMainTitles")]
        [HttpGet]
        public IQueryable<MainTitle> GetMainTitles()
        {
            return Context.Titles;
        }
        [Route("GetStatuses")]
        [HttpGet]
        public IQueryable<StatusRequest> GetStatuses()
        {
            return Context.Statuses;
        }
        [Route("GetMainRequest")]
        [HttpGet]
        public MainPage GetMainRequest()
        {
            return Context.MainPage.First(item => item.Id == 9);
        }
        [Route("AddRequest")]
        [HttpPost]
        public void AddRequest([FromBody] Request new_request)
        {
            Context.Requests.Add(new_request);
            Context.SaveChanges();
        }
        [Route("GetCountRequests")]
        [HttpGet]
        public int CountRequests()
        {
            return Context.Requests.Count();
        }
        [Route("GetRequestsNow")]
        [HttpGet]
        public Request GetRequestsNow([FromBody] string requestId)
        {
            return Context.Requests.First(i => i.Id.ToString() == requestId);
        }
        [Route("SaveNewStatusRequest")]
        [HttpPatch("SaveNewStatusRequest")]
        public void SaveNewStatusRequest([FromBody] Request reqNow)
        {
            Request requestNow = Context.Requests.First(i => i.Id == reqNow.Id);
            requestNow.StatusId = reqNow.StatusId;
            Context.SaveChanges();
        }
    }
}