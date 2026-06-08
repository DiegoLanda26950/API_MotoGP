using Microsoft.AspNetCore.Mvc;
using MotoGP_API.Models.DTOs;
using MotoGP_API.Services;

namespace MotoGP_API.Controllers
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

        // POST api/auth/login
        // Hace login y devuelve un token JWT
        [HttpPost("login")]
        public IActionResult Login(LoginDtoIn loginDtoIn)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState); // Comprueba que los datos son validos
                var token = _authService.Login(loginDtoIn);
                return Ok(new { token });
            }
            catch (KeyNotFoundException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al generar el token: " + ex.Message);
            }
        }

        // POST api/auth/register
        // Registra un nuevo usuario y devuelve un token JWT
        [HttpPost("register")]
        public IActionResult Register(UserDtoIn userDtoIn)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var token = _authService.Register(userDtoIn);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return BadRequest("Error al registrar el usuario: " + ex.Message);
            }
        }
    }
}