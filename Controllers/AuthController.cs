using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using mi_tension_backend.Models;
using mi_tension_backend.Services;
using mi_tension_backend.DTOs.Usuario;
using mi_tension_backend.DTOs;

namespace mi_tension_backend.Controllers
{
    /// <summary>
    /// Controlador encargado de la autenticación y gestión de usuarios.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AuthController(
            UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _configuration = configuration;
        }

        /// <summary>
        /// Registra un nuevo usuario en el sistema y envía un correo de confirmación.
        /// </summary>
        /// <param name="dto">Datos del usuario a registrar.</param>
        /// <returns>Resultado de la operación de registro.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistroUsuarioDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var usuarioExistente = await _userManager.FindByEmailAsync(dto.Email);
            if (usuarioExistente != null)
                return BadRequest(new { mensaje = "El correo ya está registrado." });

            var usuario = new Usuario
            {
                UserName = dto.Email,
                Email = dto.Email,
                Nombre = dto.Nombre,
                Apellidos = dto.Apellidos,
                FechaNacimiento = dto.FechaNacimiento,
                Sexo = dto.Sexo,
                TomaMedicacion = dto.TomaMedicacion
            };

            var resultado = await _userManager.CreateAsync(usuario, dto.Password);
            if (!resultado.Succeeded)
                return BadRequest(new { errores = resultado.Errors.Select(e => e.Description) });

            // Generar código de 6 dígitos
            var random = new Random();
            var codigo = random.Next(100000, 999999).ToString();
            
            usuario.CodigoVerificacion = codigo;
            usuario.CodigoVerificacionExpiracion = DateTime.UtcNow.AddMinutes(15);
            await _userManager.UpdateAsync(usuario);

            Console.WriteLine($"[AuthController] Usuario creado. Código de verificación generado: {codigo}");

         try
{
    var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "ConfirmEmailTemplate.html");
    var htmlBody = await System.IO.File.ReadAllTextAsync(templatePath);
    htmlBody = htmlBody.Replace("{{NOMBRE}}", usuario.Nombre);
    htmlBody = htmlBody.Replace("{{CODIGO}}", codigo);

    await _emailService.SendEmailAsync(dto.Email, "Tu código de verificación - Mi Tensión", htmlBody);
    Console.WriteLine("[AuthController] Email enviado correctamente.");
}
catch (Exception emailEx)
{
    Console.WriteLine($"[AuthController] ERROR al enviar email: {emailEx.Message}");
    Console.WriteLine($"[AuthController] InnerException: {emailEx.InnerException?.Message}");
}
            return Ok(new { 
                mensaje = "Registro exitoso. Revisa tu correo para obtener el código de verificación.",
                email = dto.Email 
            });
        }

        /// <summary>
        /// Verifica el código de 6 dígitos enviado al correo del usuario.
        /// </summary>
        [HttpPost("verificar-codigo")]
        public async Task<IActionResult> VerificarCodigo([FromBody] VerificarCodigoDto dto)
        {
            var usuario = await _userManager.FindByEmailAsync(dto.Email);
            if (usuario == null)
                return NotFound(new { mensaje = "Usuario no encontrado." });

            if (usuario.EmailConfirmed)
                return BadRequest(new { mensaje = "El correo ya ha sido confirmado." });

            if (usuario.CodigoVerificacion != dto.Codigo)
                return BadRequest(new { mensaje = "El código es incorrecto." });

            if (usuario.CodigoVerificacionExpiracion < DateTime.UtcNow)
                return BadRequest(new { mensaje = "El código ha expirado. Solicita uno nuevo." });

            usuario.EmailConfirmed = true;
            usuario.CodigoVerificacion = null;
            usuario.CodigoVerificacionExpiracion = null;

            var resultado = await _userManager.UpdateAsync(usuario);
            if (!resultado.Succeeded)
                return BadRequest(new { errores = resultado.Errors.Select(e => e.Description) });

            return Ok(new { mensaje = "Correo verificado exitosamente. Ya puedes iniciar sesión." });
        }

        /// <summary>
        /// Inicia sesión de un usuario y devuelve un token JWT.
        /// </summary>
        /// <param name="dto">Credenciales de inicio de sesión.</param>
        /// <returns>Token JWT e información resumida del usuario.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] IniciarSesionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var usuario = await _userManager.FindByEmailAsync(dto.Email);
            if (usuario == null)
                return Unauthorized(new { mensaje = "Credenciales incorrectas." });

            if (!usuario.EmailConfirmed)
                return Unauthorized(new { mensaje = "Debes confirmar tu correo antes de iniciar sesión." });

            var resultado = await _signInManager.CheckPasswordSignInAsync(usuario, dto.Password, lockoutOnFailure: false);
            if (!resultado.Succeeded)
                return Unauthorized(new { mensaje = "Credenciales incorrectas." });

            var token = GenerarJwt(usuario);

            return Ok(new
            {
                token,
                usuario = new
                {
                    usuario.Id,
                    usuario.Email,
                    usuario.Nombre,
                    usuario.Apellidos,
                    usuario.Sexo,
                    usuario.FechaNacimiento,
                    usuario.TomaMedicacion
                }
            });
        }
        /// <summary>
        /// Genera un token JWT para un usuario autenticado.
        /// </summary>
        /// <param name="usuario">Usuario para el que se genera el token.</param>
        /// <returns>Token JWT en formato string.</returns>
        private string GenerarJwt(Usuario usuario)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("nombre", usuario.Nombre),
                new Claim("apellidos", usuario.Apellidos)
            };

            var tokenJwt = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(int.Parse(jwtSettings["ExpirationHours"]!)),
                signingCredentials: credenciales
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenJwt);
        }
    }
}
