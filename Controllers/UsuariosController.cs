using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mi_tension_backend.Data;
using mi_tension_backend.Models;
using mi_tension_backend.DTOs;
using mi_tension_backend.DTOs.Usuario;
using Microsoft.AspNetCore.Authorization;


namespace mi_tension_backend.Controllers
{
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

        // ===============================
        // GET: api/Usuarios
        // ===============================
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

        // ===============================
        // GET: api/Usuarios/{id}
        // ===============================
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

        // ===============================
        // POST: api/Usuarios
        // ===============================
        [HttpPost]
        public async Task<ActionResult<UsuarioResponseDto>> PostUsuario([FromBody] RegistroUsuarioDto registroDto)
        {
            // Verificar si el email ya existe
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

            // Generar un Id manual si no usas Identity
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

        // ===============================
        // PUT: api/Usuarios/{id}
        // ===============================
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

        // ===============================
        // DELETE: api/Usuarios/{id}
        // ===============================
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

