using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UMA_SYSTEM.Backend.Data;
using UMA_SYSTEM.Backend.Models;

namespace UMA_SYSTEM.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParametrosController : ControllerBase
    {
        private readonly DataContext _context;

        public ParametrosController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _context.Parametros.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(Parametro parametro)
        {
            _context.Add(parametro);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var parametro = await _context.Parametros
                .SingleOrDefaultAsync(c => c.Id == id);
            if (parametro == null)
            {
                return NotFound();
            }
            return Ok(parametro);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(int id, [FromBody] Parametro parametro)
        {
            if (id != parametro.Id)
            {
                return BadRequest();
            }

            _context.Update(parametro);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var parametro = await _context.Parametros.FindAsync(id);
            if (parametro == null)
            {
                return NotFound();
            }
            _context.Remove(parametro);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
