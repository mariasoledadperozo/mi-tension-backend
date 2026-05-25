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

        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioResponseDto>> GetUsuario(string id)
        {
            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null) return NotFound();

            var response = new UsuarioResponseDto
            {
                Id              = usuario.Id,
                Email           = usuario.Email,
                Nombre          = usuario.Nombre,
                Apellidos       = usuario.Apellidos,
                FechaNacimiento = usuario.FechaNacimiento,
                Sexo            = usuario.Sexo,
                TomaMedicacion  = usuario.TomaMedicacion
            };

            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(string id, [FromBody] UpdateUsuarioDto updateDto)
        {
            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null) return NotFound();

            usuario.Nombre          = updateDto.Nombre;
            usuario.Apellidos       = updateDto.Apellidos;
            usuario.FechaNacimiento = updateDto.FechaNacimiento;
            usuario.Sexo            = updateDto.Sexo;
            usuario.TomaMedicacion  = updateDto.TomaMedicacion ?? false;

            await _context.SaveChangesAsync();
            return NoContent();
        }

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