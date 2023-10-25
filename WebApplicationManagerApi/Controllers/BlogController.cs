using ApplicationManager_ClassLibrary.Entitys;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq;
using WebApplicationManagerApi.ContextFolder;
using WebApplicationManagerApi.Models;

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
        [Route("GetBlogs")]
        [HttpGet]
        public BlogsModel GetBlogs()
        {
            IQueryable<Blog> blogs = Context.Blogs;
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string uploadPath = Path.Combine(currentDirectory, "Images");
            List<Blog_with_image> blog_s = new List<Blog_with_image>();
            foreach (Blog blog_now in blogs)
            {
                string FilePath = Path.Combine(uploadPath, blog_now.ImageUrl);
                blog_s.Add(new Blog_with_image()
                {
                    Id = blog_now.Id,
                    Description = blog_now.Description,
                    Title = blog_now.Title,
                    Created = blog_now.Created,
                    Image_name = blog_now.ImageUrl,
                    Image_byte = System.IO.File.ReadAllBytes(FilePath),
                });
            }
            BlogsModel model = new()
            {
                Name_page = Context.MainPage.First(i => i.Id == 4).Value,
                Blogs = blog_s,
            };
            return model;
        }
        [Route("GetBlog")]
        [HttpGet("id")]
        public Blog GetBlog(int id)
        {
            return Context.Blogs.FirstOrDefault(i => i.Id == id);
        }
        [Route("GetBlogModel")]
        [HttpGet("id")]
        public BlogModel GetBlogModel(int id)
        {
            Blog blog_now = Context.Blogs.FirstOrDefault(i => i.Id == id);
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string uploadPath = Path.Combine(currentDirectory, "Images");
            string FilePath = Path.Combine(uploadPath, blog_now.ImageUrl);
            Blog_with_image blog_model = new()
            {
                Id = blog_now.Id,
                Title = blog_now.Title,
                Description = blog_now.Description,
                Created = blog_now.Created,
                Image_name = blog_now.ImageUrl,
                Image_byte = System.IO.File.ReadAllBytes(FilePath),
            };
            BlogModel model = new()
            {
                Name_page = Context.MainPage.First(i => i.Id == 4).Value,
                Blog_With_Image = blog_model,
            };
            return model;
        }
        [HttpPost("AddBlog")]
        public async Task<IActionResult> AddBlogAsync()
        {
            try
            {
                var form = Request.ReadFormAsync().Result;
                var new_blog_json = form["new_blog"];
                Blog new_blog = JsonConvert.DeserializeObject<Blog>(new_blog_json);
                IFormFile image = form.Files.GetFile("image");
                // Сохранение изображения
                if (image != null && image.Length > 0)
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

                    new_blog.ImageUrl = UniqueName;
                }
                else
                {
                    new_blog.ImageUrl = "/Default/default.png"; //имя по умолчанию
                }
                Context.Blogs.Add(new_blog);
                Context.SaveChanges();
                // Вернуть успешный результат
                return Ok("Данные успешно обработаны.");
            }
            catch (Exception ex)
            {
                // Вернуть ошибку в случае исключения
                return BadRequest($"Произошла ошибка: {ex.Message}");
            }
        }

        [HttpPost("EditBlog")]
        public async Task<IActionResult> EditBlogAsync()
        {
            try
            {
                var form = Request.ReadFormAsync().Result;
                var edit_blog_json = form["edit_blog"];
                Blog edit_blog = JsonConvert.DeserializeObject<Blog>(edit_blog_json);
                IFormFile image = form.Files.GetFile("image");
                // Сохранение изображения
                if (image != null && image.Length > 0)
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
                    //сохранение новых заголовков
                    var rowsModified = Context.Database.ExecuteSqlRaw(
                   $"UPDATE [Blogs] SET Title = N'{edit_blog.Title}', " +
                   $" Description = N'{edit_blog.Description}', ImageUrl = N'{UniqueName}' WHERE Id = {edit_blog.Id}");
                }
                else
                {
                    var rowsModified = Context.Database.ExecuteSqlRaw(
                    $"UPDATE [Blogs] SET Title = N'{edit_blog.Title}', " +
                    $" Description = N'{edit_blog.Description}' WHERE Id = {edit_blog.Id}");
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

        [Route("DeleteBlog")]
        [HttpDelete("id")]
        public IActionResult DeleteBlog(int id)
        {
            Context.Blogs.Remove(GetBlog(id));
            Context.SaveChanges();
            return Ok();
        }
    }
}
