using ApplicationManager_ClassLibrary.Entitys;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text;
using WebApplicationManagerApi.ContextFolder;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApplicationManagerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MainController : ControllerBase
    {

        private readonly ApplicationDbContext Context;
        private readonly ILogger<MainController> _logger;
        private readonly IWebHostEnvironment webHost;

        public MainController(ILogger<MainController> logger, ApplicationDbContext context, IWebHostEnvironment WebHost)
        {
            Context = context;
            _logger = logger;
            webHost = WebHost;
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
        [Route("EditMain")]
        [HttpPost("EditMain")]
        public IActionResult EditMain([FromForm] MainForm form, [FromForm] IFormFile image)
        {
            try
            {
                // Обработка экземпляра класса MainForm
                // form содержит переданные данные формы (включая JSON)

                // Сохранение изображения
                if (image != null && image.Length > 0)
                {
                    string uploadPath =
                    Path.Combine(webHost.WebRootPath, "Image");
                    string UniqueName = Guid.NewGuid().ToString() + "_" + image.FileName;
                    string FilePath = Path.Combine(uploadPath, UniqueName);
                    image.CopyTo(new FileStream(FilePath, FileMode.Create));

                    var sql = $@"
                            UPDATE MainPage SET Value = CASE
                                WHEN Id = 6 THEN N'{form.ButtonTitle}'
                                WHEN Id = 7 THEN N'{form.Title}'
                                WHEN Id = 8 THEN N'{UniqueName}'
                                WHEN Id = 9 THEN N'{form.RequestTitle}'
                                ELSE Value
                            END
                            WHERE Id IN (6, 7, 8, 9)";

                    var rowsModified = Context.Database.ExecuteSqlRaw(sql);
                }
                else
                {
                    var sql = $@"
                            UPDATE MainPage SET Value = CASE
                                WHEN Id = 6 THEN N'{form.ButtonTitle}'
                                WHEN Id = 7 THEN N'{form.Title}'
                                WHEN Id = 9 THEN N'{form.RequestTitle}'
                                ELSE Value
                            END
                            WHERE Id IN (6, 7, 9)";

                    var rowsModified = Context.Database.ExecuteSqlRaw(sql);
                }

                // Вернуть успешный результат
                return Ok("Данные успешно обработаны.");
            }
            catch (Exception ex)
            {
                // Вернуть ошибку в случае исключения
                return BadRequest($"Произошла ошибка: {ex.Message}");
            }
        }
        [Route("SaveNamePages")]
        [HttpPost]
        public IActionResult SaveNamePages([FromBody] List<MainPage> names, [FromBody] List<MainPage> NamesAdmin)
        {
            try
            {
                if (names.Count > 0 && NamesAdmin.Count > 0)
                {
                    for (int i = 0; i < names.Count; i++)
                    {
                        MainPage name_admin = NamesAdmin.Find(item => item.Name.Contains(names[i].Name));
                        name_admin.Value = $"Ред. \"{names[i].Value}\"";
                    }
                    StringBuilder queryBuilder = new StringBuilder();

                    for (int i = 0; i < names.Count; i++)
                    {
                        queryBuilder.Append($"UPDATE MainPage SET Value = CASE Id WHEN {names[i].Id} THEN N'{names[i].Value}' ELSE Value END;");
                    }
                    for (int i = 0; i < NamesAdmin.Count; i++)
                    {
                        queryBuilder.Append($"UPDATE MainPage SET Value = CASE Id WHEN {NamesAdmin[i].Id} THEN N'{NamesAdmin[i].Value}' ELSE Value END;");
                    }


                    var query = queryBuilder.ToString();
                    var rowsModified = Context.Database.ExecuteSqlRaw(query);
                    return Ok("Данные успешно обработаны.");
                }
                return BadRequest($"Произошла ошибка: не найдены новые названия страниц");
            }
            catch (Exception ex)
            {
                return BadRequest($"Произошла ошибка: {ex.Message}");
            }
        }
    }
}