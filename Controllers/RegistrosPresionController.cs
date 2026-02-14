using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mi_tension_backend.Models;
using mi_tension_backend.Data;        
namespace mi_tension_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrosPresionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RegistrosPresionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/RegistrosPresion
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RegistroPresion>>> GetRegistroPresion()
        {
            return await _context.RegistroPresion.ToListAsync();
        }

        // GET: api/RegistrosPresion/5
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

        // PUT: api/RegistrosPresion/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRegistroPresion(int id, RegistroPresion registroPresion)
        {
            if (id != registroPresion.Id)
            {
                return BadRequest();
            }

            _context.Entry(registroPresion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RegistroPresionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/RegistrosPresion
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RegistroPresion>> PostRegistroPresion(RegistroPresion registroPresion)
        {
            _context.RegistroPresion.Add(registroPresion);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRegistroPresion", new { id = registroPresion.Id }, registroPresion);
        }

        // DELETE: api/RegistrosPresion/5
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
            return _context.RegistroPresion.Any(e => e.Id == id);
        }
    }
}
