    public class EstadisticasPresion
    {
        public int TotalRegistros     { get; set; }
        public int PromedioSistolica  { get; set; }
        public int PromedioDiastolica { get; set; }
        public int PromedioPulso      { get; set; }

        public int RegistrosNormales  { get; set; }
        public int RegistrosBien      { get; set; }
        public int RegistrosAltos     { get; set; }
        public int RegistrosMuyAltos  { get; set; }

        public ClasificacionPresion? UltimaClasificacion { get; set; }
    }