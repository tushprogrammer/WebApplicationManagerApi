using ApplicationManager_ClassLibrary.Entitys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationManagerApi.ContextFolder;

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
        [Route("GetService")]
        [HttpGet("{id}")]
        public Service GetService(int id)
        {
            return Context.Services.FirstOrDefault(i => i.Id == id);
        }
        [Route("AddService")]
        [HttpPost]
        public async Task<IActionResult> AddService([FromBody] Service newService)
        {
            try
            {
                await Context.Services.AddAsync(newService);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Произошла ошибка: {ex.Message}");
            }
        }
        [Route("DeleteService")]
        [HttpDelete]
        public IActionResult DeleteService(int id)
        {
            try
            {
                Context.Remove(GetService(id));
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
