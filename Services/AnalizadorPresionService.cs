// Author: María Soledad Perozo
using mi_tension_backend.Enums;
using mi_tension_backend.Models;

namespace mi_tension_backend.Services
{
    /// <summary>
    /// Servicio para analizar la presión arterial según estándares médicos,
    /// personalizado por edad, sexo y medicación del usuario.
    /// </summary>
    public class AnalizadorPresionService
    {
        // ─────────────────────────────────────────────────────────────────────
        // Métodos públicos
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Analiza un registro de presión con contexto del usuario.
        /// Ajusta umbrales y mensajes según edad, sexo y medicación.
        /// </summary>
        public ClasificacionPresion Analizar(RegistroPresion registro, Usuario usuario)
        {
            return Analizar(registro.Sistolica, registro.Diastolica, usuario);
        }

        /// <summary>
        /// Analiza valores de presión arterial sistólica y diastólica con contexto del usuario.
        /// </summary>
        public ClasificacionPresion Analizar(int sistolica, int diastolica, Usuario usuario)
        {
            int edad            = CalcularEdad(usuario.FechaNacimiento);
            bool esMujer        = usuario.Sexo == Sexo.Femenino;
            bool tomaMedicacion = usuario.TomaMedicacion == true;

            var umbrales = ObtenerUmbrales(usuario);

            // ── Crisis hipertensiva ──────────────────────────────────────────
            if (sistolica >= umbrales.CrisisSistolica || diastolica >= umbrales.CrisisDiastolica)
            {
                string mensajeCrisis = tomaMedicacion
                    ? "Sus valores indican una presión arterial muy alta a pesar de la medicación. " +
                      "Se recomienda buscar atención médica lo antes posible."
                    : "Sus valores indican una presión arterial muy alta. " +
                      "Busque atención médica de inmediato.";

                if (edad >= 65)
                    mensajeCrisis += " En personas mayores es importante actuar rápidamente para evitar posibles complicaciones.";

                return new ClasificacionPresion
                {
                    Categoria              = CategoriaPresion.MuyAlta,
                    Descripcion            = "Muy alta",
                    Mensaje                = mensajeCrisis,
                    RequiereAtencionMedica = true,
                    Sistolica              = sistolica,
                    Diastolica             = diastolica
                };
            }

            // ── Hipertensión ────────────────────────────────────────────────
            if (sistolica >= umbrales.AltaSistolica || diastolica >= umbrales.AltaDiastolica)
            {
                string mensajeAlta = tomaMedicacion
                    ? "Su presión arterial se encuentra alta a pesar de la medicación. " +
                      "Consulte estos valores con su médico."
                    : "Su presión arterial está por encima de lo recomendado. " +
                      "Es recomendable realizar un seguimiento médico.";

                if (esMujer && edad >= 50)
                {
                    mensajeAlta += " En esta etapa es especialmente importante mantener un control regular de la tensión arterial.";
                }
                else if (!esMujer && edad >= 50)
                {
                    mensajeAlta += " A partir de cierta edad es importante controlar la presión arterial con regularidad.";
                }

                return new ClasificacionPresion
                {
                    Categoria              = CategoriaPresion.Alta,
                    Descripcion            = "Alta",
                    Mensaje                = mensajeAlta,
                    RequiereAtencionMedica = true,
                    Sistolica              = sistolica,
                    Diastolica             = diastolica
                };
            }

            // ── Presión elevada / prehipertensión ───────────────────────────
            if (sistolica >= umbrales.ElevadaSistolica || diastolica >= umbrales.ElevadaDiastolica)
            {
                string mensajeElevada = "Sus valores se encuentran ligeramente elevados. " +
                    "Intente descansar, reducir el estrés y mantener hábitos saludables.";

                if (tomaMedicacion)
                    mensajeElevada += " Si estos valores se repiten con frecuencia, consulte con su médico.";

                if (edad < 18)
                    mensajeElevada = "Sus valores se encuentran algo elevados para su edad. " +
                        "Se recomienda comentarlo con un profesional de salud.";

                return new ClasificacionPresion
                {
                    Categoria              = CategoriaPresion.Bien,
                    Descripcion            = "Ligeramente elevada",
                    Mensaje                = mensajeElevada,
                    RequiereAtencionMedica = false,
                    Sistolica              = sistolica,
                    Diastolica             = diastolica
                };
            }

            // ── Normal ──────────────────────────────────────────────────────
            string mensajeNormal = tomaMedicacion
                ? "Su presión arterial se encuentra estable y dentro de un rango adecuado."
                : "Su presión arterial se encuentra dentro de un rango saludable.";

            if (edad >= 65 && !tomaMedicacion)
                mensajeNormal += " Mantenga un control periódico para seguir monitoreando sus valores.";

            return new ClasificacionPresion
            {
                Categoria              = CategoriaPresion.Normal,
                Descripcion            = "Normal",
                Mensaje                = mensajeNormal,
                RequiereAtencionMedica = false,
                Sistolica              = sistolica,
                Diastolica             = diastolica
            };
        }

        /// <summary>
        /// Obtiene estadísticas agregadas de los registros del usuario.
        /// </summary>
        public EstadisticasPresion ObtenerEstadisticas(IEnumerable<RegistroPresion> registros, Usuario usuario)
        {
            var lista = registros.ToList();

            if (!lista.Any())
                return new EstadisticasPresion();

            var clasificaciones = lista
                .Select(r => Analizar(r, usuario))
                .ToList();

            return new EstadisticasPresion
            {
                TotalRegistros      = lista.Count,
                PromedioSistolica   = (int)lista.Average(r => r.Sistolica),
                PromedioDiastolica  = (int)lista.Average(r => r.Diastolica),
                PromedioPulso       = lista.Any(r => r.Pulso > 0)
                                        ? (int)lista.Where(r => r.Pulso > 0).Average(r => r.Pulso)
                                        : 0,

                RegistrosNormales   = clasificaciones.Count(c => c.Categoria == CategoriaPresion.Normal),
                RegistrosBien       = clasificaciones.Count(c => c.Categoria == CategoriaPresion.Bien),
                RegistrosAltos      = clasificaciones.Count(c => c.Categoria == CategoriaPresion.Alta),
                RegistrosMuyAltos   = clasificaciones.Count(c => c.Categoria == CategoriaPresion.MuyAlta),

                UltimaClasificacion = clasificaciones.LastOrDefault()
            };
        }

        // ─────────────────────────────────────────────────────────────────────
        // Helpers privados
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Calcula la edad en años completos a partir de la fecha de nacimiento.
        /// </summary>
        private static int CalcularEdad(DateOnly fechaNacimiento)
        {
            var hoy  = DateOnly.FromDateTime(DateTime.UtcNow);
            int edad = hoy.Year - fechaNacimiento.Year;

            if (fechaNacimiento.AddYears(edad) > hoy)
                edad--;

            return edad;
        }

        /// <summary>
        /// Calcula los umbrales de presión arterial personalizados para un usuario.
        /// </summary>
        private static UmbralesPresion ObtenerUmbrales(Usuario usuario)
        {
            int edad       = CalcularEdad(usuario.FechaNacimiento);
            bool esMujer   = usuario.Sexo == Sexo.Femenino;
            bool tomaMed   = usuario.TomaMedicacion ?? false;

            var umbralBase = ObtenerUmbralesBase(edad, esMujer, tomaMed);

            var recientes = usuario.RegistrosPresion?
                .Where(r => r.Fecha >= DateTime.UtcNow.AddDays(-7))
                .ToList() ?? new List<RegistroPresion>();

            if (recientes.Count >= 5)
            {
                var mediaSis = recientes.Average(r => r.Sistolica);
                var mediaDia = recientes.Average(r => r.Diastolica);

                return AjustarPorTendencia(umbralBase, mediaSis, mediaDia);
            }

            return umbralBase;
        }

        /// <summary>
        /// Genera los umbrales base según edad, sexo y uso de medicación.
        /// </summary>
        private static UmbralesPresion ObtenerUmbralesBase(int edad, bool esMujer, bool tomaMed)
        {
            if (edad < 18)
                return new UmbralesPresion(160, 110, 130, 85, 115, 75);

            int crisisSis = 180;
            int crisisDia = 120;

            int altaSis = 140;
            int altaDia = 90;

            int elevadaSis = esMujer ? 118 : 120;
            int elevadaDia = esMujer ? 78 : 80;

            if (edad >= 65)
            {
                altaSis = 150;
                elevadaSis = 130;
            }

            if (tomaMed)
            {
                altaSis -= 5;
                altaDia -= 5;

                elevadaSis -= 3;
                elevadaDia -= 3;
            }

            return new UmbralesPresion(
                crisisSis,
                crisisDia,
                altaSis,
                altaDia,
                elevadaSis,
                elevadaDia
            );
        }

        /// <summary>
        /// Ajusta los umbrales en función de la tendencia reciente del usuario.
        /// </summary>
        private static UmbralesPresion AjustarPorTendencia(
            UmbralesPresion baseUmbral,
            double mediaSis,
            double mediaDia)
        {
            int ajusteSis = 0;
            int ajusteDia = 0;

            if (mediaSis >= baseUmbral.ElevadaSistolica)
                ajusteSis -= 5;

            if (mediaDia >= baseUmbral.ElevadaDiastolica)
                ajusteDia -= 5;

            if (mediaSis >= baseUmbral.AltaSistolica)
                ajusteSis -= 5;

            if (mediaDia >= baseUmbral.AltaDiastolica)
                ajusteDia -= 5;

            ajusteSis = Math.Max(ajusteSis, -10);
            ajusteDia = Math.Max(ajusteDia, -10);

            return new UmbralesPresion(
                baseUmbral.CrisisSistolica,
                baseUmbral.CrisisDiastolica,
                baseUmbral.AltaSistolica + ajusteSis,
                baseUmbral.AltaDiastolica + ajusteDia,
                baseUmbral.ElevadaSistolica + ajusteSis,
                baseUmbral.ElevadaDiastolica + ajusteDia
            );
        }
    }
}