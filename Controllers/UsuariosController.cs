// Author: María Soledad Perozo
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mi_tension_backend.Data;
using mi_tension_backend.Models;
using mi_tension_backend.DTOs;
using mi_tension_backend.DTOs.Usuario;
using Microsoft.AspNetCore.Authorization;

namespace mi_tension_backend.Controllers
{
    /// <summary>
    /// Controlador para la gestión de datos maestros de usuarios.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsuariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene la lista completa de usuarios registrados.
        /// </summary>
        /// <returns>Lista de usuarios.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioResponseDto>>> GetUsuarios()
        {
            var usuarios = await _context.Usuario.ToListAsync();

            var response = usuarios.Select(u => new UsuarioResponseDto
            {
                Id = u.Id,
                Email = u.Email,
                Nombre = u.Nombre,
                Apellidos = u.Apellidos,
                FechaNacimiento = u.FechaNacimiento,
                Sexo = u.Sexo,
                TomaMedicacion = u.TomaMedicacion
            });

            return Ok(response);
        }

        /// <summary>
        /// Obtiene la información detallada de un usuario por su ID.
        /// </summary>
        /// <param name="id">ID del usuario.</param>
        /// <returns>Datos del usuario.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioResponseDto>> GetUsuario(string id)
        {
            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null) return NotFound();

            var response = new UsuarioResponseDto
            {
                Id = usuario.Id,
                Email = usuario.Email,
                Nombre = usuario.Nombre,
                Apellidos = usuario.Apellidos,
                FechaNacimiento = usuario.FechaNacimiento,
                Sexo = usuario.Sexo,
                TomaMedicacion = usuario.TomaMedicacion
            };

            return Ok(response);
        }

        /// <summary>
        /// Crea un nuevo usuario de forma manual (sin flujo de Identity).
        /// </summary>
        /// <param name="registroDto">Datos del usuario.</param>
        /// <returns>El usuario creado.</returns>
        [HttpPost]
        public async Task<ActionResult<UsuarioResponseDto>> PostUsuario([FromBody] RegistroUsuarioDto registroDto)
        {
            if (_context.Usuario.Any(u => u.Email == registroDto.Email))
            {
                return BadRequest(new { message = "El email ya está registrado" });
            }

            var usuario = new Usuario
            {
                Email = registroDto.Email,
                Nombre = registroDto.Nombre,
                Apellidos = registroDto.Apellidos,
                FechaNacimiento = registroDto.FechaNacimiento,
                Sexo = registroDto.Sexo,
                TomaMedicacion = registroDto.TomaMedicacion
            };

            usuario.Id = Guid.NewGuid().ToString();

            _context.Usuario.Add(usuario);
            await _context.SaveChangesAsync();

            var response = new UsuarioResponseDto
            {
                Id = usuario.Id,
                Email = usuario.Email,
                Nombre = usuario.Nombre,
                Apellidos = usuario.Apellidos,
                FechaNacimiento = usuario.FechaNacimiento,
                Sexo = usuario.Sexo,
                TomaMedicacion = usuario.TomaMedicacion
            };

            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, response);
        }

        /// <summary>
        /// Actualiza la información personal de un usuario.
        /// </summary>
        /// <param name="id">ID del usuario.</param>
        /// <param name="updateDto">Nuevos datos personales.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(string id, [FromBody] RegistroUsuarioDto updateDto)
        {
            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null) return NotFound();

            usuario.Nombre = updateDto.Nombre;
            usuario.Apellidos = updateDto.Apellidos;
            usuario.FechaNacimiento = updateDto.FechaNacimiento;
            usuario.Sexo = updateDto.Sexo;
            usuario.TomaMedicacion = updateDto.TomaMedicacion;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Elimina un usuario y todos sus datos asociados.
        /// </summary>
        /// <param name="id">ID del usuario a eliminar.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(string id)
        {
            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null) return NotFound();

            _context.Usuario.Remove(usuario);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

