using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UMA_SYSTEM.Backend.Data;
using UMA_SYSTEM.Backend.Models;

namespace UMA_SYSTEM.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnexosController : ControllerBase
    {
        private readonly DataContext _context;

        public AnexosController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _context.Anexos
                .Include(s => s.Denuncia)
                .ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(Anexo anexo)
        {
            var denuncia = _context.Denuncias.FirstOrDefault(e => e.Id == anexo.IdDenuncia);

            if (denuncia != null)
            {
                anexo.Denuncia = denuncia;
                anexo.Id = 0;
                _context.Add(anexo);
                await _context.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return BadRequest("Anexo no puede ser nulo");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var anexos = await _context.Anexos
                .Include(x => x.Denuncia)
                .Where(c => c.IdDenuncia == id)
                .ToListAsync();

            if (!anexos.Any())
            {
                return NotFound();
            }
            return Ok(anexos);
        }
    }
}

