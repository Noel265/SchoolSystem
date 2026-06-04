using Microsoft.AspNetCore.Mvc;
using SchoolSystem.API.DTOs;
using SchoolSystem.API.Services;


namespace SchoolSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FullName) ||
                string.IsNullOrWhiteSpace(dto.Email) || 
                string.IsNullOrWhiteSpace(dto.Password) ||
                string.IsNullOrWhiteSpace(dto.Role))
                return BadRequest(new { message = "All fields are required"});

            var validRoles = new[] { "Admin", "Teacher", "Parent" };
            if (!validRoles.Contains(dto.Role))
                return BadRequest(new { message = "Role must be Admin, Teacher, or Parent." });

             var result = await _authService.RegisterAsync(dto);

            if (result == null)
                return Conflict(new { message = "A user with this email already exists." });

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest(new { message = "Email and password are required." });

            var result = await _authService.LoginAsync(dto);

            if (result == null)
                return Unauthorized(new { message = "Invalid email or password." });

            return Ok(result);
        }

        //Temprary
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new { message = "API is working!" });
        }
    }
}