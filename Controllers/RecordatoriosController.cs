// Author: María Soledad Perozo
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mi_tension_backend.Data;
using mi_tension_backend.Models;
using mi_tension_backend.DTOs.Recordatorio;
using Microsoft.AspNetCore.Authorization;

namespace mi_tension_backend.Controllers
{
    /// <summary>
    /// Controlador para la gestión de recordatorios de medicación.
    /// </summary>
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

        /// <summary>
        /// Obtiene la lista completa de recordatorios.
        /// </summary>
        /// <returns>Lista de recordatorios.</returns>
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

        /// <summary>
        /// Obtiene un recordatorio específico por su ID.
        /// </summary>
        /// <param name="id">ID del recordatorio.</param>
        /// <returns>Detalles del recordatorio.</returns>
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

        /// <summary>
        /// Obtiene todos los recordatorios asociados a un usuario específico.
        /// </summary>
        /// <param name="usuarioId">ID del usuario.</param>
        /// <returns>Lista de recordatorios del usuario.</returns>
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

        /// <summary>
        /// Actualiza un recordatorio existente.
        /// </summary>
        /// <param name="id">ID del recordatorio a actualizar.</param>
        /// <param name="updateDto">Nuevos datos del recordatorio.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRecordatorio(int id, UpdateRecordatorioDto updateDto)
        {
            var recordatorio = await _context.Recordatorio.FindAsync(id);

            if (recordatorio == null)
            {
                return NotFound();
            }

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

        /// <summary>
        /// Crea un nuevo recordatorio de medicación.
        /// </summary>
        /// <param name="createDto">Datos del nuevo recordatorio.</param>
        /// <returns>El recordatorio creado.</returns>
        [HttpPost]
        public async Task<ActionResult<RecordatorioResponseDto>> PostRecordatorio(CreateRecordatorioDto createDto)
        {
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

        /// <summary>
        /// Alterna el estado activo/inactivo de un recordatorio.
        /// </summary>
        /// <param name="id">ID del recordatorio.</param>
        /// <returns>Resultado de la operación.</returns>
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

        /// <summary>
        /// Elimina un recordatorio del sistema.
        /// </summary>
        /// <param name="id">ID del recordatorio a eliminar.</param>
        /// <returns>Resultado de la operación.</returns>
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
