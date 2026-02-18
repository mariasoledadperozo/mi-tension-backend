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

        // ── REGISTRO ─────────────────────────────────────────────
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

            // Generar token de confirmación
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(usuario);
            var tokenCodificado = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var urlConfirmacion = Url.Action(
                "ConfirmarEmail", "Auth",
                new { userId = usuario.Id, token = tokenCodificado },
                Request.Scheme);

            var htmlBody = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h2 style='color: #2c3e50;'>¡Bienvenida a Mi Tensión!</h2>
                    <p>Hola <strong>{usuario.Nombre}</strong>, confirma tu cuenta haciendo clic en el botón:</p>
                    <a href='{HtmlEncoder.Default.Encode(urlConfirmacion!)}' 
                       style='display:inline-block; padding:12px 24px; background:#3498db; 
                              color:white; text-decoration:none; border-radius:6px; margin:16px 0;'>
                        Verificar correo
                    </a>
                    <p style='color:#7f8c8d; font-size:13px;'>Este enlace expira en 24 horas.<br>
                    Si no creaste esta cuenta, ignora este mensaje.</p>
                </div>";

            await _emailService.SendEmailAsync(dto.Email, "Confirma tu cuenta en Mi Tensión", htmlBody);

            return Ok(new { mensaje = "Registro exitoso. Revisa tu correo para confirmar tu cuenta." });
        }

        // ── CONFIRMAR EMAIL ───────────────────────────────────────
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmarEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                return BadRequest(new { mensaje = "Enlace de confirmación inválido." });

            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario == null)
                return NotFound(new { mensaje = "Usuario no encontrado." });

            if (usuario.EmailConfirmed)
                return Ok(new { mensaje = "El correo ya estaba confirmado. Puedes iniciar sesión." });

            var tokenDecodificado = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var resultado = await _userManager.ConfirmEmailAsync(usuario, tokenDecodificado);

            if (!resultado.Succeeded)
                return BadRequest(new { mensaje = "El enlace es inválido o ha expirado." });

            return Ok(new { mensaje = "¡Correo verificado! Ya puedes iniciar sesión." });
        }

        // ── LOGIN ─────────────────────────────────────────────────
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

        // ── REENVIAR CONFIRMACIÓN ─────────────────────────────────
        [HttpPost("resend-confirmation")]
        public async Task<IActionResult> ReenviarConfirmacion([FromBody] string email)
        {
            var usuario = await _userManager.FindByEmailAsync(email);

            // Respuesta genérica por seguridad (no revela si el email existe)
            if (usuario == null || usuario.EmailConfirmed)
                return Ok(new { mensaje = "Si el correo existe y no está confirmado, recibirás un nuevo enlace." });

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(usuario);
            var tokenCodificado = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var urlConfirmacion = Url.Action(
                "ConfirmarEmail", "Auth",
                new { userId = usuario.Id, token = tokenCodificado },
                Request.Scheme);

            var htmlBody = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px;'>
                    <h2>Nuevo enlace de verificación</h2>
                    <p>Hola <strong>{usuario.Nombre}</strong>, aquí tienes un nuevo enlace:</p>
                    <a href='{HtmlEncoder.Default.Encode(urlConfirmacion!)}' 
                       style='display:inline-block; padding:12px 24px; background:#3498db; 
                              color:white; text-decoration:none; border-radius:6px;'>
                        Verificar correo
                    </a>
                </div>";

            await _emailService.SendEmailAsync(email, "Nuevo enlace de verificación - Mi Tensión", htmlBody);

            return Ok(new { mensaje = "Si el correo existe y no está confirmado, recibirás un nuevo enlace." });
        }

        // ── GENERAR JWT ───────────────────────────────────────────
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