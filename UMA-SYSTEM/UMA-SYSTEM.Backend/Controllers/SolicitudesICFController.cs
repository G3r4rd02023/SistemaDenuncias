using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UMA_SYSTEM.Backend.Data;
using UMA_SYSTEM.Backend.Models;

namespace UMA_SYSTEM.Backend.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class SolicitudesICFController : ControllerBase
    {
        private readonly DataContext _context;

        public SolicitudesICFController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _context.SolicitudesICF
                .Include(s => s.Estado)
                .ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(SolicitudICF solicitud)
        {
            var estado = _context.Estados.FirstOrDefault(e => e.Id == 3);

            if (estado != null)
            {
                solicitud.Estado = estado;
                _context.Add(solicitud);
                await _context.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return BadRequest("Solicitud no puede ser nulo");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var solicitud = await _context.SolicitudesICF
                .Include(x => x.Estado)
                .SingleOrDefaultAsync(c => c.Id == id);
            if (solicitud == null)
            {
                return NotFound();
            }
            return Ok(solicitud);
        }
    }
}