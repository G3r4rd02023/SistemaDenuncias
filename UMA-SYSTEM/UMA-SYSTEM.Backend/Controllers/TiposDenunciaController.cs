using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UMA_SYSTEM.Backend.Data;
using UMA_SYSTEM.Backend.Models;

namespace UMA_SYSTEM.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TiposDenunciaController : ControllerBase
    {
        private readonly DataContext _context;

        public TiposDenunciaController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _context.TiposDenuncia.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(TipoDenuncia tipo)
        {
            _context.Add(tipo);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
