using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpPost]
        public async Task<IActionResult> PostAsync(Usuario usuario)
        {
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
        public async Task<IActionResult> PutAsync(int id, [FromBody] Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return BadRequest();
            }

            _context.Update(usuario);
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

    }
}
