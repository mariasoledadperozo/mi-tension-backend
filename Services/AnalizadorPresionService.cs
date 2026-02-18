using mi_tension_backend.Models;

namespace mi_tension_backend.Services
{
    /// <summary>
    /// Servicio para analizar presión arterial según estándares médicos
    /// </summary>
    public class AnalizadorPresionService
    {
        /// <summary>
        /// Analiza un registro de presión y devuelve la clasificación correspondiente
        /// </summary>
        public ClasificacionPresion Analizar(RegistroPresion registro)
        {
            return Analizar(registro.Sistolica, registro.Diastolica);
        }

        /// <summary>
        /// Analiza valores de presión arterial y devuelve la clasificación
        /// </summary>
        public ClasificacionPresion Analizar(int sistolica, int diastolica)
        {
            // MUY ALTA - Crisis hipertensiva (requiere atención médica inmediata)
            if (sistolica >= 180 || diastolica >= 120)
            {
                return new ClasificacionPresion
                {
                    Categoria = CategoriaPresion.MuyAlta,
                    Descripcion = "Crisis Hipertensiva",
                    Mensaje = "¡ATENCIÓN! Presión arterial muy alta. Consulte a un médico inmediatamente.",
                    Color = "#D32F2F",
                    RequiereAtencionMedica = true,
                    Sistolica = sistolica,
                    Diastolica = diastolica
                };
            }

            // ALTA - Hipertensión Estadio 2
            if (sistolica >= 140 || diastolica >= 90)
            {
                return new ClasificacionPresion
                {
                    Categoria = CategoriaPresion.Alta,
                    Descripcion = "Hipertensión Estadio 2",
                    Mensaje = "Presión arterial alta. Consulte con su médico para evaluación y tratamiento.",
                    Color = "#F57C00",
                    RequiereAtencionMedica = true,
                    Sistolica = sistolica,
                    Diastolica = diastolica
                };
            }

            // BIEN - Hipertensión Estadio 1 o Elevada
            if (sistolica >= 120 || diastolica >= 80)
            {
                string descripcion = sistolica >= 130 || diastolica >= 80 
                    ? "Hipertensión Estadio 1" 
                    : "Presión Elevada";
                
                return new ClasificacionPresion
                {
                    Categoria = CategoriaPresion.Bien,
                    Descripcion = descripcion,
                    Mensaje = "Presión arterial en rango de precaución. Considere cambios en el estilo de vida y monitoreo regular.",
                    Color = "#FDD835",
                    RequiereAtencionMedica = false,
                    Sistolica = sistolica,
                    Diastolica = diastolica
                };
            }

            // NORMAL - Óptima
            return new ClasificacionPresion
            {
                Categoria = CategoriaPresion.Normal,
                Descripcion = "Presión Normal",
                Mensaje = "¡Excelente! Su presión arterial está en el rango normal.",
                Color = "#4CAF50",
                RequiereAtencionMedica = false,
                Sistolica = sistolica,
                Diastolica = diastolica
            };
        }

        /// <summary>
        /// Obtiene estadísticas de presión arterial de múltiples registros
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

    /// <summary>
    /// Categorías de clasificación de presión arterial
    /// </summary>
    public enum CategoriaPresion
    {
        Normal,
        Bien,
        Alta,
        MuyAlta
    }

    /// <summary>
    /// Resultado de la clasificación de presión arterial
    /// </summary>
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

    /// <summary>
    /// Estadísticas agregadas de presión arterial
    /// </summary>
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