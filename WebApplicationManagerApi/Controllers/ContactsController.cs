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
        public async Task<IQueryable<SocialNet_with_image>> GetSocialNetAsync()
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
                    Image_byte = await System.IO.File.ReadAllBytesAsync(FilePath),
                });
            }
            return nets_s.AsQueryable();
        }
        [Route("GetContactsModel")]
        [HttpGet]
        public async Task<ContactsModel> GetContactsModel()
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
            IQueryable<SocialNet_with_image> nets = await GetSocialNetAsync();
            ContactsModel model = new()
            {
                Contacts = Context.Contacts.Where(i => i.Id != 1),
                Nets = nets, 
                Name_page = Context.MainPage.First(i => i.Id == 5).Value,
                Image_name = image_address_name,
                Image_byte = await System.IO.File.ReadAllBytesAsync(FilePath_address)
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
                var form = await Request.ReadFormAsync();
                var contactsJson = form["contacts"];
                var image = form.Files.GetFile("image");

                var edit_contacts = JsonConvert.DeserializeObject<List<Contacts>>(contactsJson);


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
                Context.Contacts.RemoveRange(oldContacts.Where(i => i.Id != 1));                
                Context.Contacts.AddRange(edit_contacts);

                Context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Произошла ошибка: {ex.Message}");
            }
        }
        [Route("SaveSocialNets")]
        [HttpPost]
        public async Task<IActionResult> SaveSocialNetsAsync()
        {
            try
            {
                //files.filename == socialnetswithimage.image_name
                var form = await Request.ReadFormAsync();
                var socialNetsJson = form["SocialNets"];
                var files = form.Files.GetFiles("files");
                var edit_socialNets = JsonConvert.DeserializeObject<List<SocialNet_with_image>>(socialNetsJson);
                List<SocialNet> edit_socialnets = new List<SocialNet>();
                
                if (files != null)
                {
                    foreach (IFormFile item in files)
                    {
                        SocialNet_with_image Net_now = edit_socialNets.First(i => i.Image_name == item.FileName);
                        string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                        string uploadPath = Path.Combine(currentDirectory, "Images");
                        string UniqueName = Guid.NewGuid().ToString() + "_" + item.FileName;
                        Net_now.Image_name = UniqueName;
                        string FilePath = Path.Combine(uploadPath, UniqueName);
                        using (var fileStream = new FileStream(FilePath, FileMode.Create))
                        {
                            // Асинхронно копируем содержимое файла в поток
                            await item.CopyToAsync(fileStream);
                        }
                    }
                

                }
                foreach (SocialNet_with_image item in edit_socialNets)
                {
                    SocialNet new_net = new SocialNet();
                    new_net.Id = item.Id;
                    new_net.ImageUrl = item.Image_name;
                    new_net.Url = item.Url;
                    edit_socialnets.Add(new_net);
                }

                var oldSocialNets = Context.SocialNets;
                Context.SocialNets.RemoveRange(oldSocialNets);
                Context.SocialNets.AddRange(edit_socialnets);
                Context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Произошла ошибка: {ex.Message}");
            }
        }
    }
}
