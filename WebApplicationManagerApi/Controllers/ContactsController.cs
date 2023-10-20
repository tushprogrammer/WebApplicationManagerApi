using ApplicationManager_ClassLibrary.Entitys;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebApplicationManagerApi.ContextFolder;
using static System.Net.Mime.MediaTypeNames;

namespace WebApplicationManagerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly ApplicationDbContext Context;
        private readonly IWebHostEnvironment webHost;
        public ContactsController(ApplicationDbContext context, IWebHostEnvironment WebHost)
        {
            Context = context;
            webHost = WebHost;
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

        [Route("SaveContacts")]
        [HttpPost]
        public IActionResult SaveContacts()
        //public IActionResult SaveContacts([FromBody] List<Contacts> edit_contacts, [FromBody] List<SocialNet> edit_socialNets, [FromBody] IFormFile image)
        {
            try
            {
                var form = Request.ReadFormAsync().Result;
                var contactsJson = form["contacts"];
                var socialNetsJson = form["socialNets"];
                var image = form.Files.GetFile("image");

                // Десериализуйте JSON данные
                var edit_contacts = JsonConvert.DeserializeObject<List<Contacts>>(contactsJson);
                var edit_socialNets = JsonConvert.DeserializeObject<List<SocialNet>>(socialNetsJson);


                if (image != null)
                {
                    string uploadPath =
                    Path.Combine(webHost.WebRootPath, "Images");
                    string UniqueName = Guid.NewGuid().ToString() + "_" + image.FileName;
                    string FilePath = Path.Combine(uploadPath, UniqueName);
                    image.CopyTo(new FileStream(FilePath, FileMode.Create));

                    var rowsModified = Context.Database.ExecuteSqlRaw(
                       $"UPDATE [Contacts] SET Description = N'{UniqueName}' WHERE Id = 1");

                }
                var oldContacts = Context.Contacts;
                var oldSocialNets = Context.SocialNets;
                Context.SocialNets.RemoveRange(oldSocialNets);
                Context.Contacts.RemoveRange(oldContacts.Where(i => i.Id != 1)); //удалить все элементы в таблице кроме первой картинки


                Context.SocialNets.AddRange(edit_socialNets);
                Context.Contacts.AddRange(edit_contacts);

                Context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Произошла ошибка: {ex.Message}");
            }
        }
        [Route("SaveNewImageSocialNets")]
        [HttpPost]
        public IActionResult SaveNewImageSocialNets([FromBody] List<IFormFile> files)
        {
            try
            {
                if (files != null)
                //if (Request.Form.Files != null)
                {
                    foreach (IFormFile item in files)
                    {
                        string uploadPath =
                        Path.Combine(webHost.WebRootPath, "Images");
                        string UniqueName = Guid.NewGuid().ToString() + "_" + item.FileName;
                        string FilePath = Path.Combine(uploadPath, UniqueName);
                        item.CopyTo(new FileStream(FilePath, FileMode.Create));
                    }
                
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Произошла ошибка: {ex.Message}");
            }
        }
    }
}
