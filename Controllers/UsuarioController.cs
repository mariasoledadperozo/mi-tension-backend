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
    public class UsuariosController : ControllerBase
    {
        private readonly MiDbContext _context;

        public UsuariosController(MiDbContext context)
        {
            _context = context;
        }

        // GET: api/Usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            // Retorna la lista de todos los usuarios registrados
            return await _context.Usuarios.ToListAsync();
        }

        // GET: api/Usuarios/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(Guid id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound(new { mensaje = "Usuario no encontrado" });
            }

            return usuario;
        }

        // PUT: api/Usuarios/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(Guid id, Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return BadRequest(new { mensaje = "El ID no coincide con el usuario" });
            }

            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
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

        // POST: api/Usuarios
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            // 1. Validación: Evitar correos duplicados
            if (await _context.Usuarios.AnyAsync(u => u.Email == usuario.Email))
            {
                return BadRequest(new { mensaje = "El correo electrónico ya está registrado." });
            }

            // 2. Seguridad: Forzamos un nuevo ID (Guid) para evitar errores de clave duplicada
            usuario.Id = Guid.NewGuid();

            // 3. Opcional: Aseguramos que la fecha de creación sea la actual si no viene una
            if (usuario.FechaCreacion == default)
            {
                usuario.FechaCreacion = DateTime.UtcNow;
            }

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, usuario);
        }

        // DELETE: api/Usuarios/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(Guid id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UsuarioExists(Guid id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}