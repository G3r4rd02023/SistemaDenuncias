using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UMA_SYSTEM.Backend.Data;
using UMA_SYSTEM.Backend.Models;

namespace UMA_SYSTEM.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BitacoraController : ControllerBase
    {

        private readonly DataContext _context;

        public BitacoraController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _context.Bitacora
                .Include(b => b.Usuario)
                .Include(b => b.Objeto)
                .ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(Bitacora bitacora)
        {
            _context.Add(bitacora);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
