// Author: María Soledad Perozo
using mi_tension_backend.Models;
namespace mi_tension_backend.Services
{
    /// <summary>
    /// Servicio para analizar la presión arterial según estándares médicos simplificados.
    /// </summary>
    public class AnalizadorPresionService
    {
        /// <summary>
        /// Analiza un registro de presión y devuelve la clasificación correspondiente.
        /// </summary>
        public ClasificacionPresion Analizar(RegistroPresion registro)
        {
            return Analizar(registro.Sistolica, registro.Diastolica);
        }

        /// <summary>
        /// Analiza valores de presión arterial sistólica y diastólica.
        /// </summary>
        public ClasificacionPresion Analizar(int sistolica, int diastolica)
        {
            if (sistolica >= 180 || diastolica >= 120)
            {
                return new ClasificacionPresion
                {
                    Categoria = CategoriaPresion.MuyAlta,
                    Descripcion = "Presión muy alta",
                    Mensaje = "¡ATENCIÓN! Su presión está en un nivel de riesgo. Busque atención médica de inmediato.",
                    Color = "#D32F2F",
                    RequiereAtencionMedica = true,
                    Sistolica = sistolica,
                    Diastolica = diastolica
                };
            }
            if (sistolica >= 140 || diastolica >= 90)
            {
                return new ClasificacionPresion
                {
                    Categoria = CategoriaPresion.Alta,
                    Descripcion = "Presión alta",
                    Mensaje = "Su presión está por encima de lo normal. Consulte con su médico.",
                    Color = "#F57C00",
                    RequiereAtencionMedica = true,
                    Sistolica = sistolica,
                    Diastolica = diastolica
                };
            }
            if (sistolica >= 120 || diastolica >= 80)
            {
                return new ClasificacionPresion
                {
                    Categoria = CategoriaPresion.Bien,
                    Descripcion = "Presión un poco elevada",
                    Mensaje = "Su presión está algo elevada. Intente reducir el estrés, la sal y haga ejercicio regular.",
                    Color = "#FDD835",
                    RequiereAtencionMedica = false,
                    Sistolica = sistolica,
                    Diastolica = diastolica
                };
            }
            return new ClasificacionPresion
            {
                Categoria = CategoriaPresion.Normal,
                Descripcion = "Presión normal",
                Mensaje = "¡Muy bien! Su presión arterial está en un rango saludable.",
                Color = "#4CAF50",
                RequiereAtencionMedica = false,
                Sistolica = sistolica,
                Diastolica = diastolica
            };
        }

        /// <summary>
        /// Obtiene estadísticas agregadas de un conjunto de registros de presión arterial.
        /// </summary>
        public EstadisticasPresion ObtenerEstadisticas(IEnumerable<RegistroPresion> registros)
        {
            var lista = registros.ToList();

            if (!lista.Any())
            {
                return new EstadisticasPresion();
            }
            var clasificaciones = lista.Select(r => Analizar(r)).ToList();
            return new EstadisticasPresion
            {
                TotalRegistros = lista.Count,
                PromedioSistolica = (int)lista.Average(r => r.Sistolica),
                PromedioDiastolica = (int)lista.Average(r => r.Diastolica),
                PromedioPulso = lista.Any(r => r.Pulso > 0)
                    ? (int)lista.Where(r => r.Pulso > 0).Average(r => r.Pulso)
                    : 0,

                RegistrosNormales = clasificaciones.Count(c => c.Categoria == CategoriaPresion.Normal),
                RegistrosBien = clasificaciones.Count(c => c.Categoria == CategoriaPresion.Bien),
                RegistrosAltos = clasificaciones.Count(c => c.Categoria == CategoriaPresion.Alta),
                RegistrosMuyAltos = clasificaciones.Count(c => c.Categoria == CategoriaPresion.MuyAlta),

                UltimaClasificacion = clasificaciones.LastOrDefault()
            };
        }
    }

    public enum CategoriaPresion
    {
        Normal,
        Bien,
        Alta,
        MuyAlta
    }

    public class ClasificacionPresion
    {
        public CategoriaPresion Categoria { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public bool RequiereAtencionMedica { get; set; }
        public int Sistolica { get; set; }
        public int Diastolica { get; set; }

        public string ObtenerIcono()
        {
            return Categoria switch
            {
                CategoriaPresion.Normal => "✓",
                CategoriaPresion.Bien => "⚠",
                CategoriaPresion.Alta => "⚠⚠",
                CategoriaPresion.MuyAlta => "🚨",
                _ => ""
            };
        }
    }

    public class EstadisticasPresion
    {
        public int TotalRegistros { get; set; }
        public int PromedioSistolica { get; set; }
        public int PromedioDiastolica { get; set; }
        public int PromedioPulso { get; set; }

        public int RegistrosNormales { get; set; }
        public int RegistrosBien { get; set; }
        public int RegistrosAltos { get; set; }
        public int RegistrosMuyAltos { get; set; }

        public ClasificacionPresion? UltimaClasificacion { get; set; }

        public double PorcentajeNormales => TotalRegistros > 0
            ? Math.Round((double)RegistrosNormales / TotalRegistros * 100, 1)
            : 0;

        public double PorcentajePreocupantes => TotalRegistros > 0
            ? Math.Round((double)(RegistrosAltos + RegistrosMuyAltos) / TotalRegistros * 100, 1)
            : 0;
    }
}