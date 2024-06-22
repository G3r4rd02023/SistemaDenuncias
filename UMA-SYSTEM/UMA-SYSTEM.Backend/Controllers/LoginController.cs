using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UMA_SYSTEM.Backend.Data;
using UMA_SYSTEM.Backend.Models;

namespace UMA_SYSTEM.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly DataContext _context;

        public LoginController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("Registro")]
        public async Task<IActionResult> Registro([FromBody] Usuario model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Es recomendable hacer hash de la contraseña antes de guardarla
            model.Contraseña = BCrypt.Net.BCrypt.HashPassword(model.Contraseña);

            _context.Add(model);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Usuario registrado exitosamente." });
        }

        [HttpPost("IniciarSesion")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuario = await _context.Usuarios
                .SingleOrDefaultAsync(u => u.Email == login.Email);

            if (usuario != null)
            {
                if (BCrypt.Net.BCrypt.Verify(login.Contraseña, usuario.Contraseña))
                {
                    // Aquí puedes agregar la lógica para generar un token JWT o manejar la sesión como prefieras
                    return Ok(new { Message = "Inicio de sesión exitoso." });
                }
            }

            return Unauthorized(new { Message = "Inicio de sesión fallido. Usuario o contraseña incorrectos." });
        }
    }
}
