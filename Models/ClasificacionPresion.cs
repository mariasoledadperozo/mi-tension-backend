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
                CategoriaPresion.Normal  => "✓",
                CategoriaPresion.Bien    => "⚠",
                CategoriaPresion.Alta    => "⚠⚠",
                CategoriaPresion.MuyAlta => "🚨",
                _                        => ""
            };
        }
    }