// Author: María Soledad Perozo
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mi_tension_backend.Data;
using mi_tension_backend.Models;
using mi_tension_backend.DTOs.RegistroPresion;
using mi_tension_backend.Services;
using Microsoft.AspNetCore.Authorization;
namespace mi_tension_backend.Controllers
{
    /// <summary>
    /// Controlador para la gestión de registros de presión arterial y su análisis.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RegistrosPresionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly AnalizadorPresionService _analizadorPresion;
        public RegistrosPresionController(
            ApplicationDbContext context,
            AnalizadorPresionService analizadorPresion)
        {
            _context = context;
            _analizadorPresion = analizadorPresion;
        }
        /// <summary>
        /// Obtiene todos los registros de presión almacenados.
        /// </summary>
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

        /// <summary>
        /// Obtiene un registro de presión específico por su ID.
        /// </summary>
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

        /// <summary>
        /// Realiza un análisis médico simplificado de un registro específico.
        /// </summary>
        [HttpGet("{id}/analisis")]
        public async Task<ActionResult<ClasificacionPresion>> GetAnalisisRegistro(int id)
        {
            var registro = await _context.RegistroPresion.FindAsync(id);
            if (registro == null)
            {
                return NotFound(new { mensaje = "Registro no encontrado" });
            }
            var analisis = _analizadorPresion.Analizar(registro);
            return Ok(analisis);
        }

        /// <summary>
        /// Obtiene el historial de registros de un usuario.
        /// </summary>
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

        /// <summary>
        /// Obtiene estadísticas generales del usuario en un periodo de tiempo.
        /// </summary>
        [HttpGet("usuario/{usuarioId}/estadisticas")]
        public async Task<ActionResult<EstadisticasPresion>> GetEstadisticasUsuario(
            string usuarioId,
            [FromQuery] int? dias = 30)
        {
            var fechaLimite = DateTime.UtcNow.AddDays(-(dias ?? 30));

            var registros = await _context.RegistroPresion
                .Where(r => r.UsuarioId == usuarioId && r.Fecha >= fechaLimite)
                .OrderBy(r => r.Fecha)
                .ToListAsync();
            if (!registros.Any())
            {
                return Ok(new EstadisticasPresion());
            }
            var estadisticas = _analizadorPresion.ObtenerEstadisticas(registros);
            return Ok(estadisticas);
        }

        /// <summary>
        /// Obtiene el historial de registros detallado incluyendo el análisis automático de cada uno.
        /// </summary>
        [HttpGet("usuario/{usuarioId}/historial-con-analisis")]
        public async Task<ActionResult<IEnumerable<object>>> GetHistorialConAnalisis(
            string usuarioId,
            [FromQuery] int? limite = 20)
        {
            var registros = await _context.RegistroPresion
                .Where(r => r.UsuarioId == usuarioId)
                .OrderByDescending(r => r.Fecha)
                .Take(limite ?? 20)
                .ToListAsync();
            var historial = registros.Select(r => new
            {
                registro = new
                {
                    r.Id,
                    r.Sistolica,
                    r.Diastolica,
                    r.Pulso,
                    r.Fecha,
                    r.Notas
                },
                analisis = _analizadorPresion.Analizar(r)
            }).ToList();
            return Ok(historial);
        }

        /// <summary>
        /// Actualiza un registro de presión existente y devuelve su nuevo análisis.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRegistroPresion(int id, UpdateRegistroPresionDto updateDto)
        {
            var registro = await _context.RegistroPresion.FindAsync(id);
            if (registro == null)
            {
                return NotFound();
            }
            registro.Sistolica = updateDto.Sistolica;
            registro.Diastolica = updateDto.Diastolica;
            registro.Pulso = updateDto.Pulso;
            // FIX: convertir a UTC antes de guardar
            registro.Fecha = updateDto.Fecha.HasValue
                ? DateTime.SpecifyKind(updateDto.Fecha.Value, DateTimeKind.Utc)
                : registro.Fecha;
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
            var analisis = _analizadorPresion.Analizar(registro);
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
            return Ok(new
            {
                registro = response,
                analisis = analisis
            });
        }

        /// <summary>
        /// Crea un nuevo registro de presión y realiza el análisis automático.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<object>> PostRegistroPresion(CreateRegistroPresionDto createDto)
        {
            var usuarioExists = await _context.Usuario.AnyAsync(u => u.Id == createDto.UsuarioId);
            if (!usuarioExists)
            {
                return BadRequest(new { message = "El usuario especificado no existe" });
            }

            var fechaUtc = createDto.Fecha.HasValue
                ? DateTime.SpecifyKind(createDto.Fecha.Value, DateTimeKind.Utc)
                : DateTime.UtcNow;

            var registro = new RegistroPresion
            {
                UsuarioId = createDto.UsuarioId,
                Sistolica = createDto.Sistolica,
                Diastolica = createDto.Diastolica,
                Pulso = createDto.Pulso,
                Fecha = fechaUtc,
                Notas = createDto.Notas
            };
            _context.RegistroPresion.Add(registro);
            await _context.SaveChangesAsync();
            var analisis = _analizadorPresion.Analizar(registro);
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
            return CreatedAtAction(
                "GetRegistroPresion",
                new { id = registro.Id },
                new
                {
                    registro = response,
                    analisis = analisis
                });
        }

        /// <summary>
        /// Elimina un registro de presión del sistema.
        /// </summary>
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