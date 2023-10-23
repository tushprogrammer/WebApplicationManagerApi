using ApplicationManager_ClassLibrary.Entitys;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebApplicationManagerApi.ContextFolder;
using WebApplicationManagerApi.Models;
using static System.Net.Mime.MediaTypeNames;

namespace WebApplicationManagerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : Controller
    {
        private readonly ApplicationDbContext Context;
        public ProjectController(ApplicationDbContext context)
        {
            Context = context;
        }
        [Route("GetProjects")]
        [HttpGet]
        public ProjectsModel GetProjects()
        {
            IQueryable<Project> projects = Context.Projects;
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string uploadPath = Path.Combine(currentDirectory, "Images");

            List<Project_with_image> project_s = new List<Project_with_image>();
            foreach (Project project_now in projects)
            {
                string FilePath = Path.Combine(uploadPath, project_now.ImageUrl);
                project_s.Add(new Project_with_image()
                {
                    Id = project_now.Id,
                    Description = project_now.Description,
                    NameCompany = project_now.NameCompany,
                    Title = project_now.Title,
                    Image_name = project_now.ImageUrl,
                    Image_byte = System.IO.File.ReadAllBytes(FilePath),
                });
            }
            ProjectsModel model = new()
            {
                Name_page = Context.MainPage.First(i => i.Id == 3).Value,
                Projects = project_s,
            };
            

            return model;
        }

        [HttpPost("AddProject")]
        public async Task<IActionResult> AddProjectAsync()
        {
            try
            {
                var form = Request.ReadFormAsync().Result;
                var new_project_json = form["new_project"];
                Project new_project = JsonConvert.DeserializeObject<Project>(new_project_json);
                IFormFile image = form.Files.GetFile("image");

                // Сохранение изображения
                if (image != null && image.Length > 0)
                {
                    string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    string uploadPath = Path.Combine(currentDirectory, "Images");
                    string UniqueName = Guid.NewGuid().ToString() + "_" + image.FileName;
                    string FilePath = Path.Combine(uploadPath, UniqueName);

                    //image.CopyTo(new FileStream(FilePath, FileMode.Create));
                    using (var fileStream = new FileStream(FilePath, FileMode.Create))
                    {
                        // Асинхронно копируем содержимое файла в поток
                        await image.CopyToAsync(fileStream);
                    }
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

        [Route("GetProjectModel")]
        [HttpGet("id")]
        public ProjectModel GetProjectModel(int id)
        {
            Project project_now = Context.Projects.FirstOrDefault(i => i.Id == id);
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string uploadPath = Path.Combine(currentDirectory, "Images");
            string FilePath = Path.Combine(uploadPath, project_now.ImageUrl);

            Project_with_image project_model = new()
            {
                Id = project_now.Id,
                Description = project_now.Description,
                NameCompany = project_now.NameCompany,
                Title = project_now.Title,
                Image_name = project_now.ImageUrl,
                Image_byte = System.IO.File.ReadAllBytes(FilePath),
            };
            ProjectModel model = new()
            {
                Project_with_image = project_model,
                Name_page = Context.MainPage.First(i => i.Id == 3).Value
            };

            return model;
        }
        [Route("GetProject")]
        [HttpGet]
        public Project GetProject(int id)
        {
            return Context.Projects.First(item => item.Id == id);
        }

        [HttpPost("EditProject")]
        public async Task<IActionResult> EditProjectAsync()
        {
            try
            {
                var form = Request.ReadFormAsync().Result;
                var edit_project_json = form["edit_project"];
                Project edit_project = JsonConvert.DeserializeObject<Project>(edit_project_json);
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
        [HttpDelete("id")]
        public IActionResult DeleteProject(int id)
        {
            Context.Projects.Remove(GetProject(id));
            Context.SaveChanges();
            return Ok();
        }
        
    }
}
