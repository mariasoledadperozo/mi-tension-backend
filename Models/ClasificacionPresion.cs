    public class ClasificacionPresion
    {
        public CategoriaPresion Categoria { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public bool RequiereAtencionMedica { get; set; }
        public int Sistolica { get; set; }
        public int Diastolica { get; set; }

    }