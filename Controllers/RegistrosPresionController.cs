using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mi_tension_backend.Data;
using mi_tension_backend.Models;
using mi_tension_backend.DTOs.RegistroPresion;

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
        public async Task<ActionResult<IEnumerable<RegistroPresionResponseDto>>> GetRegistroPresion()
        {
            var registros = await _context.RegistroPresion.ToListAsync();

            var response = registros.Select(r => new RegistroPresionResponseDto
            {
                Id = r.Id,
                UsuarioId = r.UsuarioId,
                Sistolica = r.Sistolica,
                Diastolica = r.Diastolica,
                Pulso = r.Pulso,
                Fecha = r.Fecha,
                Notas = r.Notas
            });

            return Ok(response);
        }

        // GET: api/RegistrosPresion/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RegistroPresionResponseDto>> GetRegistroPresion(int id)
        {
            var registro = await _context.RegistroPresion.FindAsync(id);

            if (registro == null)
            {
                return NotFound();
            }

            var response = new RegistroPresionResponseDto
            {
                Id = registro.Id,
                UsuarioId = registro.UsuarioId,
                Sistolica = registro.Sistolica,
                Diastolica = registro.Diastolica,
                Pulso = registro.Pulso,
                Fecha = registro.Fecha,
                Notas = registro.Notas
            };

            return Ok(response);
        }

        // GET: api/RegistrosPresion/usuario/{usuarioId}
        [HttpGet("usuario/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<RegistroPresionResponseDto>>> GetRegistrosPorUsuario(string usuarioId)
        {
            var registros = await _context.RegistroPresion
                .Where(r => r.UsuarioId == usuarioId)
                .OrderByDescending(r => r.Fecha)
                .ToListAsync();

            var response = registros.Select(r => new RegistroPresionResponseDto
            {
                Id = r.Id,
                UsuarioId = r.UsuarioId,
                Sistolica = r.Sistolica,
                Diastolica = r.Diastolica,
                Pulso = r.Pulso,
                Fecha = r.Fecha,
                Notas = r.Notas
            });

            return Ok(response);
        }

        // PUT: api/RegistrosPresion/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRegistroPresion(int id, UpdateRegistroPresionDto updateDto)
        {
            var registro = await _context.RegistroPresion.FindAsync(id);

            if (registro == null)
            {
                return NotFound();
            }

            // Actualizar campos
            registro.Sistolica = updateDto.Sistolica;
            registro.Diastolica = updateDto.Diastolica;
            registro.Pulso = updateDto.Pulso;
            registro.Fecha = updateDto.Fecha ?? registro.Fecha;
            registro.Notas = updateDto.Notas;

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
        [HttpPost]
        public async Task<ActionResult<RegistroPresionResponseDto>> PostRegistroPresion(CreateRegistroPresionDto createDto)
        {
            // Verificar que el usuario existe
            var usuarioExists = await _context.Usuario.AnyAsync(u => u.Id == createDto.UsuarioId);
            if (!usuarioExists)
            {
                return BadRequest(new { message = "El usuario especificado no existe" });
            }

            var registro = new RegistroPresion
            {
                UsuarioId = createDto.UsuarioId,
                Sistolica = createDto.Sistolica,
                Diastolica = createDto.Diastolica,
                Pulso = createDto.Pulso,
                Fecha = createDto.Fecha ?? DateTime.UtcNow,
                Notas = createDto.Notas
            };

            _context.RegistroPresion.Add(registro);
            await _context.SaveChangesAsync();

            var response = new RegistroPresionResponseDto
            {
                Id = registro.Id,
                UsuarioId = registro.UsuarioId,
                Sistolica = registro.Sistolica,
                Diastolica = registro.Diastolica,
                Pulso = registro.Pulso,
                Fecha = registro.Fecha,
                Notas = registro.Notas
            };

            return CreatedAtAction("GetRegistroPresion", new { id = registro.Id }, response);
        }

        // DELETE: api/RegistrosPresion/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRegistroPresion(int id)
        {
            var registro = await _context.RegistroPresion.FindAsync(id);
            if (registro == null)
            {
                return NotFound();
            }

            _context.RegistroPresion.Remove(registro);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RegistroPresionExists(int id)
        {
            return _context.RegistroPresion.Any(e => e.Id == id);
        }
    }
}
