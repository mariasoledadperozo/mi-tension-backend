// Author: María Soledad Perozo
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using mi_tension_backend.Controllers;
using mi_tension_backend.DTOs.Usuario;
using mi_tension_backend.DTOs;
using mi_tension_backend.Enums;
using mi_tension_backend.Models;
using mi_tension_backend.Services;

namespace mi_tension_backend.Tests.Controllers
{
    public class AuthControllerTests
    {
        // ── Helpers ──────────────────────────────────────────────────────────

        private static IConfiguration BuildConfiguration()
        {
            var inMemorySettings = new Dictionary<string, string?>
            {
                { "Jwt:Key",             "SuperClaveSecretaParaTestsDe256Bits!!" },
                { "Jwt:Issuer",          "mi-tension-test" },
                { "Jwt:Audience",        "mi-tension-test" },
                { "Jwt:ExpirationHours", "2" }
            };
            return new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
        }

        private static Mock<UserManager<Usuario>> BuildUserManagerMock()
        {
            var store = new Mock<IUserStore<Usuario>>();
            return new Mock<UserManager<Usuario>>(
                store.Object,
                null!, null!, null!, null!, null!, null!, null!, null!);
        }

        private static Mock<SignInManager<Usuario>> BuildSignInManagerMock(
            Mock<UserManager<Usuario>> userManagerMock)
        {
            var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
            var claimsFactory   = new Mock<IUserClaimsPrincipalFactory<Usuario>>();
            return new Mock<SignInManager<Usuario>>(
                userManagerMock.Object,
                contextAccessor.Object,
                claimsFactory.Object,
                null!, null!, null!, null!);
        }

        private static Usuario BuildUsuario() => new Usuario
        {
            Id              = "user-test-1",
            UserName        = "test@example.com",
            Email           = "test@example.com",
            Nombre          = "Ana",
            Apellidos       = "García",
            EmailConfirmed  = true,
            FechaNacimiento = new DateOnly(1990, 1, 1),
            Sexo            = Sexo.Femenino,
            TomaMedicacion  = false
        };

        private static async Task EnsureTemplateExistsAsync()
        {
            var templateDir  = Path.Combine(Directory.GetCurrentDirectory(), "Templates");
            Directory.CreateDirectory(templateDir);
            var templatePath = Path.Combine(templateDir, "ConfirmEmailTemplate.html");
            if (!File.Exists(templatePath))
                await File.WriteAllTextAsync(templatePath,
                    "<p>Hola {{NOMBRE}}, tu código: {{CODIGO}}</p>");
        }

        // ── REGISTER ─────────────────────────────────────────────────────────

        [Fact]
        public async Task AC01_Register_DatosValidos_RetornaOk()
        {
            // Arrange
            await EnsureTemplateExistsAsync();

            var userManagerMock   = BuildUserManagerMock();
            var signInManagerMock = BuildSignInManagerMock(userManagerMock);
            var emailServiceMock  = new Mock<IEmailService>();

            userManagerMock
                .Setup(u => u.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((Usuario?)null);

            userManagerMock
                .Setup(u => u.CreateAsync(It.IsAny<Usuario>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            userManagerMock
                .Setup(u => u.UpdateAsync(It.IsAny<Usuario>()))
                .ReturnsAsync(IdentityResult.Success);

            emailServiceMock
                .Setup(e => e.SendEmailAsync(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var controller = new AuthController(
                userManagerMock.Object,
                signInManagerMock.Object,
                emailServiceMock.Object,
                BuildConfiguration());

            var dto = new RegistroUsuarioDto
            {
                Email           = "nuevo@example.com",
                Password        = "Password123!",
                Nombre          = "Ana",
                Apellidos       = "García",
                FechaNacimiento = new DateOnly(1990, 1, 1),
                Sexo            = Sexo.Femenino,
                TomaMedicacion  = false
            };

            // Act
            var result = await controller.Register(dto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task AC02_Register_EmailDuplicado_RetornaBadRequest()
        {
            // Arrange
            var userManagerMock   = BuildUserManagerMock();
            var signInManagerMock = BuildSignInManagerMock(userManagerMock);
            var emailServiceMock  = new Mock<IEmailService>();

            userManagerMock
                .Setup(u => u.FindByEmailAsync("duplicado@example.com"))
                .ReturnsAsync(BuildUsuario());

            var controller = new AuthController(
                userManagerMock.Object,
                signInManagerMock.Object,
                emailServiceMock.Object,
                BuildConfiguration());

            var dto = new RegistroUsuarioDto
            {
                Email           = "duplicado@example.com",
                Password        = "Password123!",
                Nombre          = "Ana",
                Apellidos       = "García",
                FechaNacimiento = new DateOnly(1990, 1, 1),
                Sexo            = Sexo.Femenino
            };

            // Act
            var result = await controller.Register(dto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        // ── LOGIN ─────────────────────────────────────────────────────────────

        [Fact]
        public async Task AC03_Login_CredencialesCorrectas_RetornaOkConToken()
        {
            // Arrange
            var usuario           = BuildUsuario();
            var userManagerMock   = BuildUserManagerMock();
            var signInManagerMock = BuildSignInManagerMock(userManagerMock);
            var emailServiceMock  = new Mock<IEmailService>();

            userManagerMock
                .Setup(u => u.FindByEmailAsync(usuario.Email!))
                .ReturnsAsync(usuario);

            signInManagerMock
                .Setup(s => s.CheckPasswordSignInAsync(usuario, "Password123!", false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            var controller = new AuthController(
                userManagerMock.Object,
                signInManagerMock.Object,
                emailServiceMock.Object,
                BuildConfiguration());

            var dto = new IniciarSesionDto { Email = usuario.Email!, Password = "Password123!" };

            // Act
            var result = await controller.Login(dto);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value!.ToString().Should().Contain("token");
        }

        [Fact]
        public async Task AC04_Login_ContrasenaIncorrecta_RetornaUnauthorized()
        {
            // Arrange
            var usuario           = BuildUsuario();
            var userManagerMock   = BuildUserManagerMock();
            var signInManagerMock = BuildSignInManagerMock(userManagerMock);
            var emailServiceMock  = new Mock<IEmailService>();

            userManagerMock
                .Setup(u => u.FindByEmailAsync(usuario.Email!))
                .ReturnsAsync(usuario);

            signInManagerMock
                .Setup(s => s.CheckPasswordSignInAsync(
                    usuario, It.IsAny<string>(), false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            var controller = new AuthController(
                userManagerMock.Object,
                signInManagerMock.Object,
                emailServiceMock.Object,
                BuildConfiguration());

            var dto = new IniciarSesionDto { Email = usuario.Email!, Password = "WrongPassword!" };

            // Act
            var result = await controller.Login(dto);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task AC05_Login_UsuarioInexistente_RetornaUnauthorized()
        {
            // Arrange
            var userManagerMock   = BuildUserManagerMock();
            var signInManagerMock = BuildSignInManagerMock(userManagerMock);
            var emailServiceMock  = new Mock<IEmailService>();

            userManagerMock
                .Setup(u => u.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((Usuario?)null);

            var controller = new AuthController(
                userManagerMock.Object,
                signInManagerMock.Object,
                emailServiceMock.Object,
                BuildConfiguration());

            var dto = new IniciarSesionDto
            {
                Email    = "noexiste@example.com",
                Password = "Password123!"
            };

            // Act
            var result = await controller.Login(dto);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task AC06_Login_EmailNoConfirmado_RetornaUnauthorized()
        {
            // Arrange
            var usuario          = BuildUsuario();
            usuario.EmailConfirmed = false;

            var userManagerMock   = BuildUserManagerMock();
            var signInManagerMock = BuildSignInManagerMock(userManagerMock);
            var emailServiceMock  = new Mock<IEmailService>();

            userManagerMock
                .Setup(u => u.FindByEmailAsync(usuario.Email!))
                .ReturnsAsync(usuario);

            var controller = new AuthController(
                userManagerMock.Object,
                signInManagerMock.Object,
                emailServiceMock.Object,
                BuildConfiguration());

            var dto = new IniciarSesionDto { Email = usuario.Email!, Password = "Password123!" };

            // Act
            var result = await controller.Login(dto);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        // ── VERIFICAR CÓDIGO ──────────────────────────────────────────────────

        [Fact]
        public async Task AC07_VerificarCodigo_CodigoCorrecto_RetornaOk()
        {
            // Arrange
            var usuario = BuildUsuario();
            usuario.EmailConfirmed               = false;
            usuario.CodigoVerificacion           = "123456";
            usuario.CodigoVerificacionExpiracion = DateTime.UtcNow.AddMinutes(10);

            var userManagerMock   = BuildUserManagerMock();
            var signInManagerMock = BuildSignInManagerMock(userManagerMock);
            var emailServiceMock  = new Mock<IEmailService>();

            userManagerMock
                .Setup(u => u.FindByEmailAsync(usuario.Email!))
                .ReturnsAsync(usuario);

            userManagerMock
                .Setup(u => u.UpdateAsync(It.IsAny<Usuario>()))
                .ReturnsAsync(IdentityResult.Success);

            var controller = new AuthController(
                userManagerMock.Object,
                signInManagerMock.Object,
                emailServiceMock.Object,
                BuildConfiguration());

            var dto = new VerificarCodigoDto { Email = usuario.Email!, Codigo = "123456" };

            // Act
            var result = await controller.VerificarCodigo(dto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task AC08_VerificarCodigo_CodigoExpirado_RetornaBadRequest()
        {
            // Arrange
            var usuario = BuildUsuario();
            usuario.EmailConfirmed               = false;
            usuario.CodigoVerificacion           = "123456";
            usuario.CodigoVerificacionExpiracion = DateTime.UtcNow.AddMinutes(-5);

            var userManagerMock   = BuildUserManagerMock();
            var signInManagerMock = BuildSignInManagerMock(userManagerMock);
            var emailServiceMock  = new Mock<IEmailService>();

            userManagerMock
                .Setup(u => u.FindByEmailAsync(usuario.Email!))
                .ReturnsAsync(usuario);

            var controller = new AuthController(
                userManagerMock.Object,
                signInManagerMock.Object,
                emailServiceMock.Object,
                BuildConfiguration());

            var dto = new VerificarCodigoDto { Email = usuario.Email!, Codigo = "123456" };

            // Act
            var result = await controller.VerificarCodigo(dto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}