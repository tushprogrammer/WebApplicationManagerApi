using ApplicationManager_ClassLibrary.Entitys;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationManagerApi.ContextFolder;

namespace WebApplicationManagerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly ApplicationDbContext Context;
        private readonly IWebHostEnvironment webHost;
        public BlogController(ApplicationDbContext context, IWebHostEnvironment webHost)
        {
            Context = context;
            this.webHost = webHost;
        }

        [HttpGet]
        public IQueryable<Blog> GetBlogs()
        {
            return Context.Blogs;
        }
        [Route("GetBlog")]
        [HttpGet("id")]
        public Blog GetBlog(int id)
        {
            Blog blog = Context.Blogs.FirstOrDefault(i => i.Id == id);
            return blog;
        }
        [Route("AddBlog")]
        [HttpPost("AddBlog")]
        public IActionResult AddBlog([FromForm] Blog new_blog, [FromForm] IFormFile image)
        {
            try
            {
                // Сохранение изображения
                if (image != null && image.Length > 0)
                {
                    string uploadPath =
                    Path.Combine(webHost.WebRootPath, "Image");
                    string UniqueName = Guid.NewGuid().ToString() + "_" + image.FileName;
                    string FilePath = Path.Combine(uploadPath, UniqueName);
                    image.CopyTo(new FileStream(FilePath, FileMode.Create));
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

        [Route("EditBlog")]
        [HttpPost("EditBlog")]
        public IActionResult EditBlog([FromForm] Blog edit_blog, [FromForm] IFormFile image)
        {
            try
            {
                // Сохранение изображения
                if (image != null && image.Length > 0)
                {
                    string uploadPath =
                    Path.Combine(webHost.WebRootPath, "Images");
                    string UniqueName = Guid.NewGuid().ToString() + "_" + image.FileName;
                    string FilePath = Path.Combine(uploadPath, UniqueName);
                    image.CopyTo(new FileStream(FilePath, FileMode.Create));
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
        [HttpDelete]
        public IActionResult DeleteBlog([FromBody] int id)
        {
            Context.Blogs.Remove(GetBlog(id));
            Context.SaveChanges();
            return Ok();
        }
    }
}
