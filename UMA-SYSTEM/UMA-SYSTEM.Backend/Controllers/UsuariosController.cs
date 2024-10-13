using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using UMA_SYSTEM.Backend.Data;
using UMA_SYSTEM.Backend.Models;

namespace UMA_SYSTEM.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly DataContext _context;

        public UsuariosController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _context.Usuarios.ToListAsync());
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostAsync(Usuario usuario)
        {
            usuario.Contraseña = BCrypt.Net.BCrypt.HashPassword(usuario.Contraseña);
            _context.Add(usuario);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var usuario = await _context.Usuarios
                .SingleOrDefaultAsync(c => c.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }
            return Ok(usuario);
        }

        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetByEmailAsync(string email)
        {
            var usuario = await _context.Usuarios
                .SingleOrDefaultAsync(u => u.Email == email);

            if (usuario == null)
            {
                return NotFound();
            }

            return Ok(usuario);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(int id, [FromBody] EditarUsuarioVM usuario)
        {
            if (id != usuario.Id)
            {
                return BadRequest();
            }

            var usuarioEncontrado = await _context.Usuarios.AsNoTracking().
                FirstOrDefaultAsync(u => u.Id == usuario.Id);
            if (usuarioEncontrado == null)
            {
                return NotFound();
            }

            var user = new Usuario()
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellidos = usuario.Apellidos,
                DNI = usuario.DNI,
                Contraseña = usuarioEncontrado.Contraseña,
                FechaCreacion = usuarioEncontrado.FechaCreacion,
                FechaVencimiento = usuarioEncontrado.FechaVencimiento,
                Email = usuarioEncontrado.Email,
                EstadoUsuario = usuarioEncontrado.EstadoUsuario,
                RolId = usuarioEncontrado.RolId
            };

            _context.Update(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            _context.Remove(usuario);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("CambiarPassword/{id}")]
        public async Task<IActionResult> CambiarPassword(int id, [FromBody] CambiarPasswordVM model)
        {
            if (id != model.UserId)
            {
                return BadRequest();
            }

            var usuarioEncontrado = await _context.Usuarios.AsNoTracking().
                FirstOrDefaultAsync(u => u.Id == model.UserId);
            if (usuarioEncontrado == null)
            {
                return NotFound();
            }

            usuarioEncontrado.Contraseña = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);

            _context.Update(usuarioEncontrado);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}