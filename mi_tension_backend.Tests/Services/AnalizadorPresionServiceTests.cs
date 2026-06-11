// Author: María Soledad Perozo
// Pruebas unitarias para AnalizadorPresionService
using mi_tension_backend.Enums;
using mi_tension_backend.Models;
using mi_tension_backend.Services;
using Xunit;
using FluentAssertions;

namespace mi_tension_backend.Tests.Services;

public class AnalizadorPresionServiceTests
{
    private readonly AnalizadorPresionService _sut = new();

    // ─── Helpers para crear usuarios de prueba ───────────────────────────────

    private static Usuario UsuarioBase(int edad = 30, Sexo sexo = Sexo.Masculino, bool tomaMed = false)
        => new()
        {
            FechaNacimiento  = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-edad)),
            Sexo             = sexo,
            TomaMedicacion   = tomaMed,
            RegistrosPresion = new List<RegistroPresion>()
        };

    // ─── 1. Clasificación Normal ─────────────────────────────────────────────

    [Fact]
    public void Analizar_PresionNormal_RetornaCategoriaCorrectaYSinAtencionMedica()
    {
        var usuario = UsuarioBase(edad: 35);

        var resultado = _sut.Analizar(115, 75, usuario);

        resultado.Categoria.Should().Be(CategoriaPresion.Normal);
        resultado.Descripcion.Should().Be("normal");
        resultado.Sistolica.Should().Be(115);
        resultado.Diastolica.Should().Be(75);
    }

    [Fact]
    public void Analizar_PresionNormal_ConMedicacion_MensajeMencioneEstabilidad()
    {
        var usuario = UsuarioBase(edad: 45, tomaMed: true);

        var resultado = _sut.Analizar(115, 75, usuario);

        resultado.Categoria.Should().Be(CategoriaPresion.Normal);
        resultado.Mensaje.Should().Contain("estable");
    }

    [Fact]
    public void Analizar_PresionNormal_UsuarioMayorSinMedicacion_MensajeSugiereSeguimiento()
    {
        var usuario = UsuarioBase(edad: 68);

        var resultado = _sut.Analizar(118, 75, usuario);

        resultado.Categoria.Should().Be(CategoriaPresion.Normal);
        resultado.Mensaje.Should().Contain("periódico");
    }

    // ─── 2. Presión Elevada / Prehipertensión ────────────────────────────────

    [Fact]
    public void Analizar_PresionElevada_AdultoMasculino_RetornaCategoriaBien()
    {
        var usuario = UsuarioBase(edad: 40, sexo: Sexo.Masculino);

        var resultado = _sut.Analizar(125, 82, usuario);

        resultado.Categoria.Should().Be(CategoriaPresion.Bien);
        resultado.Descripcion.Should().Be("ligeramente elevada");
    }

    [Fact]
    public void Analizar_PresionElevada_Menor18_MensajeEspecificoEdad()
    {
        var usuario = UsuarioBase(edad: 16);

        var resultado = _sut.Analizar(118, 78, usuario);

        resultado.Mensaje.Should().Contain("edad");
    }

    [Fact]
    public void Analizar_PresionElevada_ConMedicacion_MensajeSugiereMedico()
    {
        var usuario = UsuarioBase(edad: 50, tomaMed: true);

        var resultado = _sut.Analizar(119, 76, usuario);

        resultado.Categoria.Should().Be(CategoriaPresion.Bien);
        resultado.Mensaje.Should().Contain("médico");
    }

    // ─── 3. Hipertensión (Alta) ───────────────────────────────────────────────

    [Fact]
    public void Analizar_Hipertension_AdultoSinMedicacion_RetornaCategoriaAlta()
    {
        var usuario = UsuarioBase(edad: 45);

        var resultado = _sut.Analizar(145, 92, usuario);

        resultado.Categoria.Should().Be(CategoriaPresion.Alta);
        resultado.Descripcion.Should().Be("alta");
        resultado.Mensaje.Should().Contain("por encima de lo recomendado");
    }

    [Fact]
    public void Analizar_Hipertension_ConMedicacion_MensajeMencionaMedicacion()
    {
        var usuario = UsuarioBase(edad: 50, tomaMed: true);

        var resultado = _sut.Analizar(138, 87, usuario);

        resultado.Categoria.Should().Be(CategoriaPresion.Alta);
        resultado.Mensaje.Should().Contain("medicación");
    }

    [Fact]
    public void Analizar_Hipertension_MujerMayorDe50_MensajeMencionaControl()
    {
        var usuario = UsuarioBase(edad: 55, sexo: Sexo.Femenino);

        var resultado = _sut.Analizar(143, 91, usuario);

        resultado.Categoria.Should().Be(CategoriaPresion.Alta);
        resultado.Mensaje.Should().Contain("control regular");
    }

    [Fact]
    public void Analizar_Hipertension_HombreMayorDe50_MensajeMencionaEdad()
    {
        var usuario = UsuarioBase(edad: 55, sexo: Sexo.Masculino);

        var resultado = _sut.Analizar(143, 91, usuario);

        resultado.Categoria.Should().Be(CategoriaPresion.Alta);
        resultado.Mensaje.Should().Contain("edad");
    }

    // ─── 4. Crisis Hipertensiva (MuyAlta) ────────────────────────────────────

    [Fact]
    public void Analizar_CrisisHipertensiva_RetornaCategoriaMuyAlta()
    {
        var usuario = UsuarioBase(edad: 40);

        var resultado = _sut.Analizar(185, 125, usuario);

        resultado.Categoria.Should().Be(CategoriaPresion.MuyAlta);
        resultado.Descripcion.Should().Be("muy alta");
    }

    [Fact]
    public void Analizar_CrisisHipertensiva_ConMedicacion_MensajeMencionaMedicacion()
    {
        var usuario = UsuarioBase(edad: 50, tomaMed: true);

        var resultado = _sut.Analizar(185, 122, usuario);

        resultado.Categoria.Should().Be(CategoriaPresion.MuyAlta);
        resultado.Mensaje.Should().Contain("medicación");
    }

    [Fact]
    public void Analizar_CrisisHipertensiva_Mayor65_MensajeMencionaRapidamente()
    {
        var usuario = UsuarioBase(edad: 70);

        var resultado = _sut.Analizar(182, 121, usuario);

        resultado.Categoria.Should().Be(CategoriaPresion.MuyAlta);
        resultado.Mensaje.Should().Contain("rápidamente");
    }

    // ─── 5. Umbrales por edad: mayor de 65 ───────────────────────────────────

    [Fact]
    public void Analizar_Mayor65_UmbralAjustado_152SistolicaEsAlta()
    {
        var usuario = UsuarioBase(edad: 70);

        var resultado = _sut.Analizar(152, 88, usuario);

        resultado.Categoria.Should().Be(CategoriaPresion.Alta);
    }

    [Fact]
    public void Analizar_Mayor65_149SistolicaNoEsAlta()
    {
        var usuario = UsuarioBase(edad: 70);

        var resultado = _sut.Analizar(149, 79, usuario);

        resultado.Categoria.Should().NotBe(CategoriaPresion.Alta);
    }

    // ─── 6. ObtenerEstadisticas ───────────────────────────────────────────────

    [Fact]
    public void ObtenerEstadisticas_ListaVacia_RetornaEstadisticasVacias()
    {
        var usuario = UsuarioBase();

        var resultado = _sut.ObtenerEstadisticas(new List<RegistroPresion>(), usuario);

        resultado.TotalRegistros.Should().Be(0);
    }

    [Fact]
    public void ObtenerEstadisticas_ConRegistros_CalculaPromediosCorrectamente()
    {
        var usuario = UsuarioBase(edad: 40);
        var registros = new List<RegistroPresion>
        {
            new() { UsuarioId = "test", Sistolica = 120, Diastolica = 80, Pulso = 70, Fecha = DateTime.UtcNow },
            new() { UsuarioId = "test", Sistolica = 130, Diastolica = 84, Pulso = 72, Fecha = DateTime.UtcNow },
            new() { UsuarioId = "test", Sistolica = 110, Diastolica = 76, Pulso = 68, Fecha = DateTime.UtcNow },
        };

        var resultado = _sut.ObtenerEstadisticas(registros, usuario);

        resultado.TotalRegistros.Should().Be(3);
        resultado.PromedioSistolica.Should().Be(120);
        resultado.PromedioDiastolica.Should().Be(80);
        resultado.PromedioPulso.Should().Be(70);
    }

    [Fact]
    public void ObtenerEstadisticas_ClasificaRegistrosCorrectamente()
    {
        var usuario = UsuarioBase(edad: 40);
        var registros = new List<RegistroPresion>
        {
            new() { UsuarioId = "test", Sistolica = 115, Diastolica = 75, Fecha = DateTime.UtcNow },
            new() { UsuarioId = "test", Sistolica = 115, Diastolica = 75, Fecha = DateTime.UtcNow },
            new() { UsuarioId = "test", Sistolica = 125, Diastolica = 82, Fecha = DateTime.UtcNow },
            new() { UsuarioId = "test", Sistolica = 145, Diastolica = 92, Fecha = DateTime.UtcNow },
            new() { UsuarioId = "test", Sistolica = 185, Diastolica = 122, Fecha = DateTime.UtcNow },
        };

        var resultado = _sut.ObtenerEstadisticas(registros, usuario);

        resultado.RegistrosNormales.Should().Be(2);
        resultado.RegistrosBien.Should().Be(1);
        resultado.RegistrosAltos.Should().Be(1);
        resultado.RegistrosMuyAltos.Should().Be(1);
        resultado.UltimaClasificacion.Should().NotBeNull();
    }
}