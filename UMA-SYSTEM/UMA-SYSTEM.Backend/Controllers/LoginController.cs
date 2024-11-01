using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
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

            if (usuario != null && usuario.EstadoUsuario == "Activo")
            {
                if (usuario.SesionActiva)
                {
                    return Conflict(new { Message = "El usuario ya tiene una sesion activa." });
                }

                if (BCrypt.Net.BCrypt.Verify(login.Contraseña, usuario.Contraseña))
                {
                    usuario.SesionActiva = true;
                    await _context.SaveChangesAsync();

                    // Aquí puedes agregar la lógica para generar un token JWT o manejar la sesión como prefieras
                    return Ok(new { Message = "Inicio de sesión exitoso." });
                }
            }

            return Unauthorized(new { Message = "Inicio de sesión fallido. Usuario o contraseña incorrectos." });
        }

        [HttpPost("CerrarSesion")]
        public async Task<IActionResult> Logout([FromBody] LoginViewModel login)
        {
            var usuario = await _context.Usuarios
                .SingleOrDefaultAsync(u => u.Email == login.Email);

            if (usuario == null)
            {
                return NotFound();
            }

            usuario.SesionActiva = false;
            await _context.SaveChangesAsync();
            return Ok(new { Message = " La sesión se cerró exitosamente." });
        }
    }
}