using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mi_tension_backend.Data;
using mi_tension_backend.Models;
using mi_tension_backend.DTOs.Recordatorio;
using Microsoft.AspNetCore.Authorization;

namespace mi_tension_backend.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RecordatoriosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RecordatoriosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Recordatorios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecordatorioResponseDto>>> GetRecordatorio()
        {
            var recordatorios = await _context.Recordatorio.ToListAsync();

            var response = recordatorios.Select(r => new RecordatorioResponseDto
            {
                Id = r.Id,
                NombreMedicina = r.NombreMedicina,
                Dosis = r.Dosis,
                Hora = r.Hora,
                Dias = r.Dias,
                UsuarioId = r.UsuarioId,
                FechaCreacion = r.FechaCreacion,
                Activo = r.Activo
            });

            return Ok(response);
        }

        // GET: api/Recordatorios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RecordatorioResponseDto>> GetRecordatorio(int id)
        {
            var recordatorio = await _context.Recordatorio.FindAsync(id);

            if (recordatorio == null)
            {
                return NotFound();
            }

            var response = new RecordatorioResponseDto
            {
                Id = recordatorio.Id,
                NombreMedicina = recordatorio.NombreMedicina,
                Dosis = recordatorio.Dosis,
                Hora = recordatorio.Hora,
                Dias = recordatorio.Dias,
                UsuarioId = recordatorio.UsuarioId,
                FechaCreacion = recordatorio.FechaCreacion,
                Activo = recordatorio.Activo
            };

            return Ok(response);
        }

        // GET: api/Recordatorios/usuario/{usuarioId}
        [HttpGet("usuario/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<RecordatorioResponseDto>>> GetRecordatoriosPorUsuario(string usuarioId)
        {
            var recordatorios = await _context.Recordatorio
                .Where(r => r.UsuarioId == usuarioId)
                .OrderBy(r => r.Hora)
                .ToListAsync();

            var response = recordatorios.Select(r => new RecordatorioResponseDto
            {
                Id = r.Id,
                NombreMedicina = r.NombreMedicina,
                Dosis = r.Dosis,
                Hora = r.Hora,
                Dias = r.Dias,
                UsuarioId = r.UsuarioId,
                FechaCreacion = r.FechaCreacion,
                Activo = r.Activo
            });

            return Ok(response);
        }

        // PUT: api/Recordatorios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRecordatorio(int id, UpdateRecordatorioDto updateDto)
        {
            var recordatorio = await _context.Recordatorio.FindAsync(id);

            if (recordatorio == null)
            {
                return NotFound();
            }

            // Actualizar campos
            recordatorio.NombreMedicina = updateDto.NombreMedicina;
            recordatorio.Dosis = updateDto.Dosis;
            recordatorio.Hora = updateDto.Hora;
            recordatorio.Dias = updateDto.Dias;
            recordatorio.Activo = updateDto.Activo;

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
        [HttpPost]
        public async Task<ActionResult<RecordatorioResponseDto>> PostRecordatorio(CreateRecordatorioDto createDto)
        {
            // Verificar que el usuario existe
            var usuarioExists = await _context.Usuario.AnyAsync(u => u.Id == createDto.UsuarioId);
            if (!usuarioExists)
            {
                return BadRequest(new { message = "El usuario especificado no existe" });
            }

            var recordatorio = new Recordatorio
            {
                UsuarioId = createDto.UsuarioId,
                NombreMedicina = createDto.NombreMedicina,
                Dosis = createDto.Dosis,
                Hora = createDto.Hora,
                Dias = createDto.Dias,
                Activo = createDto.Activo,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Recordatorio.Add(recordatorio);
            await _context.SaveChangesAsync();

            var response = new RecordatorioResponseDto
            {
                Id = recordatorio.Id,
                NombreMedicina = recordatorio.NombreMedicina,
                Dosis = recordatorio.Dosis,
                Hora = recordatorio.Hora,
                Dias = recordatorio.Dias,
                UsuarioId = recordatorio.UsuarioId,
                FechaCreacion = recordatorio.FechaCreacion,
                Activo = recordatorio.Activo
            };

            return CreatedAtAction("GetRecordatorio", new { id = recordatorio.Id }, response);
        }

        // PUT: api/Recordatorios/5/toggle
        [HttpPut("{id}/toggle")]
        public async Task<IActionResult> ToggleRecordatorio(int id)
        {
            var recordatorio = await _context.Recordatorio.FindAsync(id);

            if (recordatorio == null)
            {
                return NotFound();
            }

            recordatorio.Activo = !recordatorio.Activo;
            await _context.SaveChangesAsync();

            return NoContent();
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
