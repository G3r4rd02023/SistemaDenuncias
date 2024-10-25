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
    public class DenunciasController : ControllerBase
    {
        private readonly DataContext _context;

        public DenunciasController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _context.Denuncias
                .Include(d => d.TipoDenuncia)
                .Include(d => d.Estado)
                .ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(DenunciaAnonimaVM denuncia)
        {
            var estado = _context.Estados.FirstOrDefault(e => e.Id == denuncia.IdEstado);
            var tipo = _context.TiposDenuncia.FirstOrDefault(e => e.Id == denuncia.IdTipoDenuncia);

            if (estado != null && tipo != null)
            {
                denuncia.TipoDenuncia = tipo;
                denuncia.Estado = estado;
                _context.Add(denuncia);
                await _context.SaveChangesAsync();
                var nuevaDenuncia = _context.Denuncias.FirstOrDefault(d => d.Id == denuncia.Id);
                var anexo = new Anexo()
                {
                    Denuncia = nuevaDenuncia,
                    IdDenuncia = nuevaDenuncia!.Id,
                    Fecha = DateTime.Now,
                    URL = denuncia.URLImagen,
                    NombreArchivo = denuncia.NombreArchivo
                };
                _context.Anexos.Add(anexo);
                await _context.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return BadRequest("Estado o tipo no pueden ser nulos");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var denuncia = await _context.Denuncias
                .Include(x => x.TipoDenuncia)
                .Include(x => x.Estado)
                .SingleOrDefaultAsync(c => c.Id == id);
            if (denuncia == null)
            {
                return NotFound();
            }
            return Ok(denuncia);
        }
    }
}