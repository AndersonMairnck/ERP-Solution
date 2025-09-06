using Microsoft.AspNetCore.Mvc;
using ERPCore.Models;
using ERPCore.Services;

namespace ERPCore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _authService.Authenticate(model.Username, model.Password);

            if (user == null)
                return Unauthorized(new { message = "Usuário ou senha inválidos" });

            var token = _authService.GenerateJwtToken(user);

            return Ok(new
            {
                user.Id,
                user.Username,
                user.FullName,
                user.Email,
                user.Role,
                Token = token
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user, [FromQuery] string password)
        {
            var result = await _authService.Register(user, password);

            if (!result)
                return BadRequest(new { message = "Usuário já existe" });

            return Ok(new { message = "Usuário criado com sucesso" });
        }
    }
}