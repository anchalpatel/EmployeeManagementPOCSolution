using EmployeeManagement.Application.ServiceInterface;
using EmployeeManagement.Core.DTO;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger _logger;

        public AccountController(IAuthService authService, ILogger<AccountController> logger)
        {
            _authService = authService;
            _logger = logger;
        }
        [HttpPost("register")]

        public async Task<IActionResult> Register([FromBody] RegisterUser model)
        {
            var token = await _authService.RegisterAsync(model, 0, "");
            if(token != null) return Ok(new { success = true, Token = token, Message = "User created successfully" });

            return BadRequest(new { success = false, Message = "Registration Failed"});
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var token = await _authService.LoginAsync(model);
            if(token != null) return Ok(new { success = true, Token = token, Message = "Logged In successfully" });
            
            return BadRequest(new { success = false, Message = "Invalid credentials" });
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var isLoggeout = await _authService.Logout();
            if (isLoggeout) return Ok(new { success = true, Message = "User signned out successfully" });

            return BadRequest(new { success = false, Message = "Error occured in logging out" });
        }

    }
}
