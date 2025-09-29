using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rechazos.Dtos;
using Rechazos.Services;
using System.Diagnostics;

namespace Rechazos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var response = await _authService.LoginAsync(request);

            if (response == null)
            {
                return Unauthorized(new { message = "Nombre, número de nómina incorrectos" });
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            try
            {
                var user = await _authService.RegisterAsync(request);

                if(user == null)
                {
                    return BadRequest(new { message = "Ya existe un usuario con ese nombre y número de nómina" });
                }

                return Ok(new { message = "Usuario registrado exitosamente" });
            }
            catch(Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Route("Refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return BadRequest("Refresh token es requerido");
            }

            var response = await _authService.RefreshTokenAsync(refreshToken);

            if(response == null)
            {
                return Unauthorized(new { message = "Refresh token inválido o expirado" });
            }

            return Ok(response);
        }

    }
}