using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mi_tension_backend.Models;
using mi_tension_backend.Context;

namespace mi_tension_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistroPresionController : ControllerBase
    {
        private readonly MiDbContext _context;

        public RegistroPresionController(MiDbContext context)
        {
            _context = context;
        }

        // GET: api/RegistroPresion
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RegistroPresion>>> GetRegistroPresion()
        {
            return await _context.RegistroPresion.ToListAsync();
        }

        // GET: api/RegistroPresion/Usuario/{userId}
        // Este es nuevo y muy útil: Obtiene todas las tensiones de UN solo usuario
        [HttpGet("Usuario/{userId}")]
        public async Task<ActionResult<IEnumerable<RegistroPresion>>> GetRegistrosPorUsuario(Guid userId)
        {
            return await _context.RegistroPresion
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.Fecha) // Las más recientes primero
                .ToListAsync();
        }

        // GET: api/RegistroPresion/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RegistroPresion>> GetRegistroPresion(int id)
        {
            var registroPresion = await _context.RegistroPresion.FindAsync(id);

            if (registroPresion == null)
            {
                return NotFound();
            }

            return registroPresion;
        }

        // POST: api/RegistroPresion
        [HttpPost]
        public async Task<ActionResult<RegistroPresion>> PostRegistroPresion(RegistroPresion registroPresion)
        {
            // 1. Validar que el usuario al que se le asigna la tensión existe
            var usuarioExiste = await _context.Usuarios.AnyAsync(u => u.Id == registroPresion.UserId);
            if (!usuarioExiste)
            {
                return BadRequest(new { mensaje = "El ID de usuario proporcionado no existe." });
            }

            // 2. Si no viene fecha, ponemos la actual
            if (registroPresion.Fecha == default)
            {
                registroPresion.Fecha = DateTime.UtcNow;
            }

            _context.RegistroPresion.Add(registroPresion);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRegistroPresion", new { id = registroPresion.id }, registroPresion);
        }

        // DELETE: api/RegistroPresion/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRegistroPresion(int id)
        {
            var registroPresion = await _context.RegistroPresion.FindAsync(id);
            if (registroPresion == null)
            {
                return NotFound();
            }

            _context.RegistroPresion.Remove(registroPresion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RegistroPresionExists(int id)
        {
            return _context.RegistroPresion.Any(e => e.id == id);
        }
    }
}