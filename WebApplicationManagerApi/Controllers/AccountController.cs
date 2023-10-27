using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplicationManagerApi.AuthApp;
using WebApplicationManagerApi.ContextFolder;

namespace WebApplicationManagerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly ILogger<MainController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AccountController(ILogger<MainController> logger,  
            UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpPost("authenticate")]
        public async Task<bool> PasswordSignInAsync()
        {
            var data = await Request.ReadFormAsync();
            string login = data["username"];
            string password = data["password"];
            var loginResult = await _signInManager.PasswordSignInAsync(login,
                password,
                false,
                lockoutOnFailure: false);
            return loginResult.Succeeded;
           
        }

    }
}
