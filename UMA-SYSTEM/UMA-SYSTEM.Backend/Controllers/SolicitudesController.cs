using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UMA_SYSTEM.Backend.Data;
using UMA_SYSTEM.Backend.Models;

namespace UMA_SYSTEM.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolicitudesController : ControllerBase
    {
        private readonly DataContext _context;

        public SolicitudesController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _context.Solicitudes
                .Include(s => s.Usuario)
                .ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(Solicitud solicitud)
        {
            var usuario = _context.Usuarios.FirstOrDefault(e => e.Id == solicitud.IdUsuario);           

            if (usuario != null)
            {
                solicitud.Usuario = usuario;               
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
            var solicitud = await _context.Solicitudes
                .Include(x => x.Usuario)                
                .SingleOrDefaultAsync(c => c.Id == id);
            if (solicitud == null)
            {
                return NotFound();
            }
            return Ok(solicitud);
        }

    }
}
