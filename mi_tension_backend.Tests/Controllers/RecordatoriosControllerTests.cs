// Author: María Soledad Perozo
// Pruebas unitarias para RecordatoriosController
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mi_tension_backend.Controllers;
using mi_tension_backend.Data;
using mi_tension_backend.DTOs.Recordatorio;
using mi_tension_backend.Enums;
using mi_tension_backend.Models;
using Xunit;
using FluentAssertions;

namespace mi_tension_backend.Tests.Controllers;

public class RecordatoriosControllerTests
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

    private static Recordatorio RecordatorioFake(int id = 1, string usuarioId = "user-1") => new()
    {
        Id             = id,
        UsuarioId      = usuarioId,
        NombreMedicina = "Enalapril",
        Dosis          = "10mg",
        Hora           = new TimeOnly(8, 0),
        Dias           = new List<DiasSemana> { DiasSemana.Lunes, DiasSemana.Miercoles },
        Activo         = true,
        FechaCreacion  = DateTime.UtcNow
    };

    // ─── GET ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetRecordatoriosPorUsuario_CuandoExisten_RetornaOkConLista()
    {
        using var context = CrearContexto("get_recordatorios_existen");
        context.Usuario.Add(UsuarioFake());
        context.Recordatorio.Add(RecordatorioFake());
        context.Recordatorio.Add(RecordatorioFake(id: 2));
        await context.SaveChangesAsync();
        var controller = new RecordatoriosController(context);

        var resultado = await controller.GetRecordatoriosPorUsuario("user-1");

        var ok = resultado.Result.Should().BeOfType<OkObjectResult>().Subject;
        var lista = ok.Value.Should().BeAssignableTo<IEnumerable<RecordatorioResponseDto>>().Subject;
        lista.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetRecordatoriosPorUsuario_CuandoNoHay_RetornaOkConListaVacia()
    {
        using var context = CrearContexto("get_recordatorios_vacio");
        context.Usuario.Add(UsuarioFake());
        await context.SaveChangesAsync();
        var controller = new RecordatoriosController(context);

        var resultado = await controller.GetRecordatoriosPorUsuario("user-1");

        var ok = resultado.Result.Should().BeOfType<OkObjectResult>().Subject;
        var lista = ok.Value.Should().BeAssignableTo<IEnumerable<RecordatorioResponseDto>>().Subject;
        lista.Should().BeEmpty();
    }

    // ─── POST ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task PostRecordatorio_ConUsuarioValido_RetornaOkConDatos()
    {
        using var context = CrearContexto("post_recordatorio_valido");
        context.Usuario.Add(UsuarioFake());
        await context.SaveChangesAsync();
        var controller = new RecordatoriosController(context);

        var createDto = new CreateRecordatorioDto
        {
            UsuarioId      = "user-1",
            NombreMedicina = "Losartán",
            Dosis          = "50mg",
            Hora           = new TimeOnly(9, 0),
            Dias           = new List<DiasSemana> { DiasSemana.Lunes },
            Activo         = true
        };

        var resultado = await controller.PostRecordatorio(createDto);

        var ok = resultado.Result.Should().BeOfType<OkObjectResult>().Subject;
        var dto = ok.Value.Should().BeAssignableTo<RecordatorioResponseDto>().Subject;
        dto.NombreMedicina.Should().Be("Losartán");
        dto.UsuarioId.Should().Be("user-1");
    }

    [Fact]
    public async Task PostRecordatorio_ConUsuarioInexistente_RetornaBadRequest()
    {
        using var context = CrearContexto("post_recordatorio_usuario_invalido");
        var controller = new RecordatoriosController(context);

        var createDto = new CreateRecordatorioDto
        {
            UsuarioId      = "usuario-no-existe",
            NombreMedicina = "Losartán",
            Dosis          = "50mg",
            Hora           = new TimeOnly(9, 0),
            Dias           = new List<DiasSemana> { DiasSemana.Lunes },
            Activo         = true
        };

        var resultado = await controller.PostRecordatorio(createDto);

        resultado.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    // ─── PUT ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task PutRecordatorio_CuandoExiste_ActualizaYRetornaNoContent()
    {
        using var context = CrearContexto("put_recordatorio_existe");
        context.Usuario.Add(UsuarioFake());
        context.Recordatorio.Add(RecordatorioFake());
        await context.SaveChangesAsync();
        var controller = new RecordatoriosController(context);

        var updateDto = new UpdateRecordatorioDto
        {
            NombreMedicina = "Enalapril Actualizado",
            Dosis          = "20mg",
            Hora           = new TimeOnly(10, 0),
            Dias           = new List<DiasSemana> { DiasSemana.Viernes },
            Activo         = false
        };

        var resultado = await controller.PutRecordatorio(1, updateDto);

        resultado.Should().BeOfType<NoContentResult>();
        var actualizado = await context.Recordatorio.FindAsync(1);
        actualizado!.NombreMedicina.Should().Be("Enalapril Actualizado");
        actualizado.Activo.Should().BeFalse();
    }

    [Fact]
    public async Task PutRecordatorio_CuandoNoExiste_RetornaNotFound()
    {
        using var context = CrearContexto("put_recordatorio_no_existe");
        var controller = new RecordatoriosController(context);

        var updateDto = new UpdateRecordatorioDto
        {
            NombreMedicina = "Test",
            Dosis          = "10mg",
            Hora           = new TimeOnly(8, 0),
            Dias           = new List<DiasSemana>(),
            Activo         = true
        };

        var resultado = await controller.PutRecordatorio(99, updateDto);

        resultado.Should().BeOfType<NotFoundResult>();
    }

    // ─── TOGGLE ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task ToggleRecordatorio_CuandoEstaActivo_DesactivaYRetornaNoContent()
    {
        using var context = CrearContexto("toggle_recordatorio_activo");
        context.Usuario.Add(UsuarioFake());
        context.Recordatorio.Add(RecordatorioFake()); // Activo = true
        await context.SaveChangesAsync();
        var controller = new RecordatoriosController(context);

        var resultado = await controller.ToggleRecordatorio(1);

        resultado.Should().BeOfType<NoContentResult>();
        var recordatorio = await context.Recordatorio.FindAsync(1);
        recordatorio!.Activo.Should().BeFalse();
    }

    [Fact]
    public async Task ToggleRecordatorio_CuandoNoExiste_RetornaNotFound()
    {
        using var context = CrearContexto("toggle_recordatorio_no_existe");
        var controller = new RecordatoriosController(context);

        var resultado = await controller.ToggleRecordatorio(99);

        resultado.Should().BeOfType<NotFoundResult>();
    }

    // ─── DELETE ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteRecordatorio_CuandoExiste_EliminaYRetornaNoContent()
    {
        using var context = CrearContexto("delete_recordatorio_existe");
        context.Usuario.Add(UsuarioFake());
        context.Recordatorio.Add(RecordatorioFake());
        await context.SaveChangesAsync();
        var controller = new RecordatoriosController(context);

        var resultado = await controller.DeleteRecordatorio(1);

        resultado.Should().BeOfType<NoContentResult>();
        var eliminado = await context.Recordatorio.FindAsync(1);
        eliminado.Should().BeNull();
    }

    [Fact]
    public async Task DeleteRecordatorio_CuandoNoExiste_RetornaNotFound()
    {
        using var context = CrearContexto("delete_recordatorio_no_existe");
        var controller = new RecordatoriosController(context);

        var resultado = await controller.DeleteRecordatorio(99);

        resultado.Should().BeOfType<NotFoundResult>();
    }
}