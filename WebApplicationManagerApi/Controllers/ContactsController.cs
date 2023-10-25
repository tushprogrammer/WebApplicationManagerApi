using ApplicationManager_ClassLibrary.Entitys;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebApplicationManagerApi.ContextFolder;
using WebApplicationManagerApi.Models;
using static System.Net.Mime.MediaTypeNames;
using static System.Reflection.Metadata.BlobBuilder;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        public IQueryable<SocialNet_with_image> GetSocialNet()
        {
            IQueryable<SocialNet> nets = Context.SocialNets;
            List<SocialNet_with_image> nets_s = new List<SocialNet_with_image>();
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string uploadPath = Path.Combine(currentDirectory, "Images");
            foreach (SocialNet net_now in nets)
            {
                string FilePath = Path.Combine(uploadPath, net_now.ImageUrl);
                nets_s.Add(new SocialNet_with_image()
                {
                    Id = net_now.Id,
                    Url = net_now.Url,
                    Image_name = net_now.ImageUrl,
                    Image_byte = System.IO.File.ReadAllBytes(FilePath),
                });
            }
            return nets_s.AsQueryable();
        }
        [Route("GetContactsModel")]
        [HttpGet]
        public ContactsModel GetContactsModel()
        {
            //IQueryable<SocialNet> nets = Context.SocialNets;
            //List<SocialNet_with_image> nets_s = new List<SocialNet_with_image>();
            //foreach (SocialNet net_now in nets)
            //{
            //    string FilePath = Path.Combine(uploadPath, net_now.ImageUrl);
            //    nets_s.Add(new SocialNet_with_image()
            //    {
            //        Id = net_now.Id,
            //        Url = net_now.Url,
            //        Image_name = net_now.ImageUrl,
            //        Image_byte = System.IO.File.ReadAllBytes(FilePath),
            //    });
            //}
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string uploadPath = Path.Combine(currentDirectory, "Images");
            string image_address_name = Context.Contacts.First(i => i.Id == 1).Description;
            string FilePath_address = Path.Combine(uploadPath, image_address_name);
            ContactsModel model = new()
            {
                Contacts = Context.Contacts.Where(i => i.Id != 1),
                Nets = GetSocialNet(),
                Name_page = Context.MainPage.First(i => i.Id == 5).Value,
                Image_name = image_address_name,
                Image_byte = System.IO.File.ReadAllBytes(FilePath_address)
            };
            return model;
        }

        [Route("SaveContacts")]
        [HttpPost]
        public async Task<IActionResult> SaveContactsAsync()
        //public IActionResult SaveContacts([FromBody] List<Contacts> edit_contacts, [FromBody] List<SocialNet> edit_socialNets, [FromBody] IFormFile image)
        {
            try
            {
                var form = Request.ReadFormAsync().Result;
                var contactsJson = form["contacts"];
                var socialNetsJson = form["socialNets"];
                var image = form.Files.GetFile("image");

                var edit_contacts = JsonConvert.DeserializeObject<List<Contacts>>(contactsJson);
                var edit_socialNets = JsonConvert.DeserializeObject<List<SocialNet>>(socialNetsJson);


                if (image != null)
                {
                    string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    string uploadPath = Path.Combine(currentDirectory, "Images");
                    string UniqueName = Guid.NewGuid().ToString() + "_" + image.FileName;
                    string FilePath = Path.Combine(uploadPath, UniqueName);
                    using (var fileStream = new FileStream(FilePath, FileMode.Create))
                    {
                        // Асинхронно копируем содержимое файла в поток
                        await image.CopyToAsync(fileStream);
                    }

                    var rowsModified =  await Context.Database.ExecuteSqlRawAsync(
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
