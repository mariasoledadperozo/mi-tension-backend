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
    public class RecordatoriosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RecordatoriosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Recordatorios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Recordatorio>>> GetRecordatorio()
        {
            return await _context.Recordatorio.ToListAsync();
        }

        // GET: api/Recordatorios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Recordatorio>> GetRecordatorio(int id)
        {
            var recordatorio = await _context.Recordatorio.FindAsync(id);

            if (recordatorio == null)
            {
                return NotFound();
            }

            return recordatorio;
        }

        // PUT: api/Recordatorios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRecordatorio(int id, Recordatorio recordatorio)
        {
            if (id != recordatorio.Id)
            {
                return BadRequest();
            }

            _context.Entry(recordatorio).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecordatorioExists(id))
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

        // POST: api/Recordatorios
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Recordatorio>> PostRecordatorio(Recordatorio recordatorio)
        {
            _context.Recordatorio.Add(recordatorio);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRecordatorio", new { id = recordatorio.Id }, recordatorio);
        }

        // DELETE: api/Recordatorios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecordatorio(int id)
        {
            var recordatorio = await _context.Recordatorio.FindAsync(id);
            if (recordatorio == null)
            {
                return NotFound();
            }

            _context.Recordatorio.Remove(recordatorio);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RecordatorioExists(int id)
        {
            return _context.Recordatorio.Any(e => e.Id == id);
        }
    }
}
