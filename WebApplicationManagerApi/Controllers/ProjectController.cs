using ApplicationManager_ClassLibrary.Entitys;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationManagerApi.ContextFolder;
using static System.Net.Mime.MediaTypeNames;

namespace WebApplicationManagerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : Controller
    {
        private readonly ApplicationDbContext Context;
        private readonly IWebHostEnvironment webHost;
        public ProjectController(ApplicationDbContext context, IWebHostEnvironment webHost)
        {
            Context = context;
            this.webHost = webHost;
        }

        [HttpGet]
        public IQueryable<Project> GetProjects()
        {
            return Context.Projects;
        }

        [Route("AddProject")]
        [HttpPost("AddProject")]
        public IActionResult AddProject([FromForm] Project new_project, [FromForm] IFormFile image)
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
                    new_project.ImageUrl = UniqueName;
                }
                else
                {
                    new_project.ImageUrl = "/Default/default.png"; //имя по умолчанию
                }
                Context.Projects.Add(new_project);
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
        [HttpGet]
        public Project GetProject([FromBody] int id)
        {
            return Context.Projects.FirstOrDefault(i => i.Id == id);
        }
        [Route("EditProject")]
        [HttpPost("EditProject")]
        public IActionResult EditProject([FromForm] Project edit_project, [FromForm] IFormFile image)
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
                        $"UPDATE [Projects] SET Title = N'{edit_project.Title}', NameCompany = N'{edit_project.NameCompany}', " +
                        $" Description = N'{edit_project.Description}', ImageUrl = N'{UniqueName}' WHERE Id = {edit_project.Id}");
                }
                else
                {
                    var rowsModified = Context.Database.ExecuteSqlRaw(
                       $"UPDATE [Projects] SET Title = N'{edit_project.Title}', NameCompany = N'{edit_project.NameCompany}', " +
                       $" Description = N'{edit_project.Description}' WHERE Id = {edit_project.Id}");
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
        [Route("DeleteProject")]
        [HttpDelete]
        public IActionResult DeleteProject([FromBody] int id)
        {
            Context.Projects.Remove(GetProject(id));
            Context.SaveChanges();
            return Ok();
        }
    }
}
