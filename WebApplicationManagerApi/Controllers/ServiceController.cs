using ApplicationManager_ClassLibrary.Entitys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationManagerApi.ContextFolder;
using WebApplicationManagerApi.Models;

namespace WebApplicationManagerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly ApplicationDbContext Context;
        public ServiceController(ApplicationDbContext context)
        {
            Context = context;
        }

        [HttpGet]
        public IQueryable<Service> GetServices()
        {
            return Context.Services;
        }
        [Route("GetServiceModel")]
        [HttpGet("id")]
        public DetailsServiceModel GetServiceModel(int id)
        {
            Service service_now = Context.Services.FirstOrDefault(i => i.Id == id);
            DetailsServiceModel model = new()
            {
                Id = service_now.Id,
                Title = service_now.Title,
                Description = service_now.Description,
                Name_page = Context.MainPage.First(i => i.Id == 2).Value,
            };
            return model; 
        }
        private Service GetService(int id)
        {
            return Context.Services.FirstOrDefault(i => i.Id == id);
        }
        [Route("AddService")]
        [HttpPost]
        public IActionResult AddService([FromBody] Service newService)
        {
            try
            {
                Context.Services.Add(newService);
                Context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Произошла ошибка: {ex.Message}");
            }
        }
        [Route("DeleteService")]
        [HttpDelete("id")]
        public IActionResult DeleteService(int id)
        {
            try
            {
                Context.Remove(GetService(id));
                Context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Произошла ошибка: {ex.Message}");
            }
        }
        [Route("EditService")]
        [HttpPost]
        public IActionResult EditService([FromBody] Service edit_service)
        {
            try
            {
                var rowsModified = Context.Database.ExecuteSqlRaw(
                  $"UPDATE [Services] SET Title = N'{edit_service.Title}', " +
                  $" Description = N'{edit_service.Description}' WHERE Id = {edit_service.Id}");
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Произошла ошибка: {ex.Message}");
            }
        }
    }
}
