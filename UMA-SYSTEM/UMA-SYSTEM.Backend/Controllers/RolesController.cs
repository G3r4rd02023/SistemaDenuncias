using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UMA_SYSTEM.Backend.Data;
using UMA_SYSTEM.Backend.Models;

namespace UMA_SYSTEM.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly DataContext _context;

        public RolesController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _context.Roles.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return Ok(role);
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(Rol rol)
        {
            _context.Add(rol);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(int id, [FromBody] Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return BadRequest();
            }

            var usuarioExistente = await _context.Usuarios.FindAsync(id);
            if (usuarioExistente == null)
            {
                return NotFound();
            }

            usuarioExistente.EstadoUsuario = usuario.EstadoUsuario;
            usuarioExistente.RolId = usuario.RolId;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var rol = await _context.Roles.FindAsync(id);
            if (rol == null)
            {
                return NotFound();
            }
            _context.Remove(rol);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}