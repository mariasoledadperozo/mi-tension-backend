// Author: María Soledad Perozo
// Pruebas unitarias para RegistrosPresionController
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mi_tension_backend.Controllers;
using mi_tension_backend.Data;
using mi_tension_backend.DTOs.RegistroPresion;
using mi_tension_backend.Enums;
using mi_tension_backend.Models;
using mi_tension_backend.Services;
using Xunit;
using FluentAssertions;

namespace mi_tension_backend.Tests.Controllers;

public class RegistrosPresionControllerTests
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
        FechaNacimiento = new DateOnly(1990, 6, 15),
        Sexo            = Sexo.Femenino,
        TomaMedicacion  = false,
        RegistrosPresion = new List<RegistroPresion>()
    };

    private static RegistroPresion RegistroFake(int id = 1, string usuarioId = "user-1") => new()
    {
        Id         = id,
        UsuarioId  = usuarioId,
        Sistolica  = 120,
        Diastolica = 80,
        Pulso      = 70,
        Fecha      = DateTime.UtcNow,
        Notas      = "Registro de prueba"
    };

    // ─── GET registros por usuario ────────────────────────────────────────────

    [Fact]
    public async Task GetRegistrosPorUsuario_CuandoExisten_RetornaOkConLista()
    {
        using var context = CrearContexto("get_registros_existen");
        var usuario = UsuarioFake();
        context.Usuario.Add(usuario);
        context.RegistroPresion.Add(RegistroFake(1));
        context.RegistroPresion.Add(RegistroFake(2));
        await context.SaveChangesAsync();
        var controller = new RegistrosPresionController(context, new AnalizadorPresionService());

        var resultado = await controller.GetRegistrosPorUsuario("user-1");

        var ok = resultado.Result.Should().BeOfType<OkObjectResult>().Subject;
        var lista = ok.Value.Should().BeAssignableTo<IEnumerable<RegistroPresionResponseDto>>().Subject;
        lista.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetRegistrosPorUsuario_CuandoNoHay_RetornaOkConListaVacia()
    {
        using var context = CrearContexto("get_registros_vacio");
        context.Usuario.Add(UsuarioFake());
        await context.SaveChangesAsync();
        var controller = new RegistrosPresionController(context, new AnalizadorPresionService());

        var resultado = await controller.GetRegistrosPorUsuario("user-1");

        var ok = resultado.Result.Should().BeOfType<OkObjectResult>().Subject;
        var lista = ok.Value.Should().BeAssignableTo<IEnumerable<RegistroPresionResponseDto>>().Subject;
        lista.Should().BeEmpty();
    }

    // ─── GET estadísticas ────────────────────────────────────────────────────

    [Fact]
    public async Task GetEstadisticasUsuario_CuandoUsuarioNoExiste_RetornaNotFound()
    {
        using var context = CrearContexto("get_estadisticas_no_existe");
        var controller = new RegistrosPresionController(context, new AnalizadorPresionService());

        var resultado = await controller.GetEstadisticasUsuario("id-inexistente");

        resultado.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetEstadisticasUsuario_SinRegistros_RetornaOkConEstadisticasVacias()
    {
        using var context = CrearContexto("get_estadisticas_sin_registros");
        context.Usuario.Add(UsuarioFake());
        await context.SaveChangesAsync();
        var controller = new RegistrosPresionController(context, new AnalizadorPresionService());

        var resultado = await controller.GetEstadisticasUsuario("user-1");

        resultado.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetEstadisticasUsuario_ConRegistros_RetornaOkConDatos()
    {
        using var context = CrearContexto("get_estadisticas_con_registros");
        context.Usuario.Add(UsuarioFake());
        context.RegistroPresion.Add(RegistroFake(1));
        context.RegistroPresion.Add(RegistroFake(2));
        await context.SaveChangesAsync();
        var controller = new RegistrosPresionController(context, new AnalizadorPresionService());

        var resultado = await controller.GetEstadisticasUsuario("user-1", dias: 30);

        var ok = resultado.Result.Should().BeOfType<OkObjectResult>().Subject;
        var estadisticas = ok.Value.Should().BeAssignableTo<EstadisticasPresion>().Subject;
        estadisticas.TotalRegistros.Should().Be(2);
    }

    // ─── POST ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task PostRegistroPresion_ConUsuarioValido_RetornaOkConAnalisis()
    {
        using var context = CrearContexto("post_registro_valido");
        context.Usuario.Add(UsuarioFake());
        await context.SaveChangesAsync();
        var controller = new RegistrosPresionController(context, new AnalizadorPresionService());

        var createDto = new CreateRegistroPresionDto
        {
            UsuarioId  = "user-1",
            Sistolica  = 118,
            Diastolica = 76,
            Pulso      = 72,
            Fecha      = DateTime.UtcNow,
            Notas      = "Prueba"
        };

        var resultado = await controller.PostRegistroPresion(createDto);

        var ok = resultado.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task PostRegistroPresion_ConUsuarioInexistente_RetornaBadRequest()
    {
        using var context = CrearContexto("post_registro_usuario_invalido");
        var controller = new RegistrosPresionController(context, new AnalizadorPresionService());

        var createDto = new CreateRegistroPresionDto
        {
            UsuarioId  = "no-existe",
            Sistolica  = 120,
            Diastolica = 80,
            Pulso      = 70,
            Fecha      = DateTime.UtcNow
        };

        var resultado = await controller.PostRegistroPresion(createDto);

        resultado.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    // ─── DELETE ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteRegistroPresion_CuandoExiste_EliminaYRetornaNoContent()
    {
        using var context = CrearContexto("delete_registro_existe");
        context.Usuario.Add(UsuarioFake());
        context.RegistroPresion.Add(RegistroFake());
        await context.SaveChangesAsync();
        var controller = new RegistrosPresionController(context, new AnalizadorPresionService());

        var resultado = await controller.DeleteRegistroPresion(1);

        resultado.Should().BeOfType<NoContentResult>();
        var eliminado = await context.RegistroPresion.FindAsync(1);
        eliminado.Should().BeNull();
    }

    [Fact]
    public async Task DeleteRegistroPresion_CuandoNoExiste_RetornaNotFound()
    {
        using var context = CrearContexto("delete_registro_no_existe");
        var controller = new RegistrosPresionController(context, new AnalizadorPresionService());

        var resultado = await controller.DeleteRegistroPresion(99);

        resultado.Should().BeOfType<NotFoundResult>();
    }
}