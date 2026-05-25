// Author: María Soledad Perozo
// Pruebas unitarias para UsuariosController
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mi_tension_backend.Controllers;
using mi_tension_backend.Data;
using mi_tension_backend.DTOs.Usuario;
using mi_tension_backend.Enums;
using mi_tension_backend.Models;
using Xunit;
using FluentAssertions;
namespace mi_tension_backend.Tests.Controllers;

public class UsuariosControllerTests
{
    private ApplicationDbContext CrearContexto(string nombreBd)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: nombreBd)
            .Options;
        return new ApplicationDbContext(options);
    }

    private static Usuario UsuarioFake(string id = "user-1") => new()
    {
        Id              = id,
        UserName        = "test@test.com",
        Email           = "test@test.com",
        Nombre          = "María",
        Apellidos       = "Perozo",
        FechaNacimiento = new DateOnly(1995, 6, 15),
        Sexo            = Sexo.Femenino,
        TomaMedicacion  = false
    };

    // ─── GET ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetUsuario_CuandoExiste_RetornaOkConDatos()
    {
        // Arrange
        using var context = CrearContexto("get_usuario_existe");
        context.Usuario.Add(UsuarioFake());
        await context.SaveChangesAsync();
        var controller = new UsuariosController(context);

        // Act
        var resultado = await controller.GetUsuario("user-1");

        // Assert
        var ok = resultado.Result.Should().BeOfType<OkObjectResult>().Subject;
        var dto = ok.Value.Should().BeAssignableTo<UsuarioResponseDto>().Subject;
        dto.Nombre.Should().Be("María");
        dto.Email.Should().Be("test@test.com");
    }

    [Fact]
    public async Task GetUsuario_CuandoNoExiste_RetornaNotFound()
    {
        using var context = CrearContexto("get_usuario_no_existe");
        var controller = new UsuariosController(context);

        var resultado = await controller.GetUsuario("id-inexistente");

        resultado.Result.Should().BeOfType<NotFoundResult>();
    }

    // ─── PUT ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task PutUsuario_CuandoExiste_ActualizaDatosYRetornaNoContent()
    {
        using var context = CrearContexto("put_usuario_existe");
        context.Usuario.Add(UsuarioFake());
        await context.SaveChangesAsync();
        var controller = new UsuariosController(context);

        var updateDto = new UpdateUsuarioDto
        {
            Nombre          = "Soledad",
            Apellidos       = "Perozo Actualizado",
            FechaNacimiento = new DateOnly(1995, 6, 15),
            Sexo            = Sexo.Femenino,
            TomaMedicacion  = true
        };

        var resultado = await controller.PutUsuario("user-1", updateDto);

        resultado.Should().BeOfType<NoContentResult>();

        var usuarioActualizado = await context.Usuario.FindAsync("user-1");
        usuarioActualizado!.Nombre.Should().Be("Soledad");
        usuarioActualizado.TomaMedicacion.Should().BeTrue();
    }

    [Fact]
    public async Task PutUsuario_CuandoNoExiste_RetornaNotFound()
    {
        using var context = CrearContexto("put_usuario_no_existe");
        var controller = new UsuariosController(context);

        var updateDto = new UpdateUsuarioDto
        {
            Nombre          = "Test",
            Apellidos       = "Test",
            FechaNacimiento = new DateOnly(1990, 1, 1),
            Sexo            = Sexo.Masculino,
            TomaMedicacion  = false
        };

        var resultado = await controller.PutUsuario("id-inexistente", updateDto);

        resultado.Should().BeOfType<NotFoundResult>();
    }

    // ─── DELETE ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteUsuario_CuandoExiste_EliminaYRetornaNoContent()
    {
        using var context = CrearContexto("delete_usuario_existe");
        context.Usuario.Add(UsuarioFake());
        await context.SaveChangesAsync();
        var controller = new UsuariosController(context);

        var resultado = await controller.DeleteUsuario("user-1");

        resultado.Should().BeOfType<NoContentResult>();
        var usuarioEliminado = await context.Usuario.FindAsync("user-1");
        usuarioEliminado.Should().BeNull();
    }

    [Fact]
    public async Task DeleteUsuario_CuandoNoExiste_RetornaNotFound()
    {
        using var context = CrearContexto("delete_usuario_no_existe");
        var controller = new UsuariosController(context);

        var resultado = await controller.DeleteUsuario("id-inexistente");

        resultado.Should().BeOfType<NotFoundResult>();
    }
}